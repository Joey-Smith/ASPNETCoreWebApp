using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using WebApplicationMVC.Areas.Administration.Models.ViewModels;
using WebApplicationMVC.Areas.Template.Extensions;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.ViewModels;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace WebApplicationMVC.Areas.Administration.Controllers
{
    [Authorize(Roles = "Administration")]
    [Area("Administration")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }
        public IActionResult Search()
        {
            var model = new SearchViewModel(_userManager, _roleManager);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Search(SearchViewModel model)
        {
            var roles = model.Roles.Where(_ => _.Selected).Select(_ => _.Text).ToList();
            model.Users = _userManager.Users
                .Where(_ => model.Username == null || _.UserName.Contains(model.Username))
                .Where(_ => model.Email == null || _.Email.Contains(model.Email))
                .ToList();

            if (roles.Any())
            {
                var users = model.Users.ToList();

                foreach (var user in model.Users)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    if (!userRoles.Intersect(roles).Any())
                    {
                        users.Remove(user);
                    }
                }

                model.Users = users;
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserViewModel();

            model.Roles = _roleManager.Roles.Select(_ => new SelectListItem() { Value = _.Id, Text = _.Name, Selected = roles.Contains(_.Name) }).ToList();
            model.Username = user.UserName;
            model.PhoneNumber = user.PhoneNumber;
            model.Disabled = user.Disabled;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                user.PhoneNumber = model.PhoneNumber;
                user.Disabled = model.Disabled;
                user.UserName = model.Username;

                await _userManager.UpdateAsync(user);

                await _userManager.AddToRolesAsync(user, model.Roles.Where(_ => _.Selected).Select(_ => _.Text).ToList());
                await _userManager.RemoveFromRolesAsync(user, model.Roles.Where(_ => !_.Selected).Select(_ => _.Text).ToList());

                TempData.Put<StatusMessageViewModel>("StatusMessage", new StatusMessageViewModel("User updated successfully.", StatusType.Success));

                return RedirectToAction("Search");
            }

            TempData.Put<StatusMessageViewModel>("StatusMessage", new StatusMessageViewModel("User update failed.", StatusType.Error));

            return View(model);
        }

        public IActionResult ConfirmMassNewAccountEmails()
        {
            MassNewAccountViewModel massNewAccountViewModel = new MassNewAccountViewModel()
            {
                //Count = _userManager.Users.Where(u => string.IsNullOrEmpty(u.PasswordHash)).Count()
                Count = _userManager.Users.Count()
            };

            return View(massNewAccountViewModel);
        }
        public IActionResult MassNewAccountEmails()
        {
            SendMassNewAccountEmails();

            TempData.Put<StatusMessageViewModel>("StatusMessage", new StatusMessageViewModel("Mass email process has begun.", StatusType.Success));

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public async void SendMassNewAccountEmails()
        {
            // This method is intended to send mass emails to users who have not yet set their password.
            //var users = _userManager.Users.Where(u => string.IsNullOrEmpty(u.PasswordHash)).ToList();
            var users = _userManager.Users.ToList();

            if (users == null || users.Count == 0)
            {
                Console.WriteLine("No users found without a password.");
                return;
            }

            Console.WriteLine($"Found {users.Count} users without a password. Sending emails...");

            try
            {
                foreach (var user in users)
                {
                    await _userManager.GeneratePasswordResetTokenAsync(user).ContinueWith(tokenTask =>
                    {
                        var token = tokenTask.Result;
                        if (!string.IsNullOrEmpty(token))
                        {
                            // Here you would typically send the email with the reset token.
                            // For example, using an email service to send the token to the user's email address.
                            // This is a placeholder for the actual email sending logic.
                            var callbackUrl = Url.Page(
                                "/Account/ResetPassword",
                                pageHandler: null,
                                values: new { token },
                                protocol: Request.Scheme);

                            _emailSender.SendEmailAsync(
                                user.Email,
                                "Reset Password",
                                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                            Console.WriteLine($"Email sent to {user.Email} with reset token: {token}");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending emails: {ex.Message}");
            }
        }
    }
}
