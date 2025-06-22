using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationMVC.Areas.Administration.Models;
using WebApplicationMVC.Areas.Administration.Models.ViewModels;
using WebApplicationMVC.Areas.Template.Extensions;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.ViewModels;

namespace WebApplicationMVC.Areas.Administration.Controllers
{
    [Authorize(Roles = "Administration")]
    [Area("Administration")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
    }
}
