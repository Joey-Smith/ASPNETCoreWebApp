using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Areas.Administration.Models.ViewModels
{
    public class SearchViewModel
    {
        public SearchViewModel() { }
        public SearchViewModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            Roles = roleManager.Roles.Select(_ => new SelectListItem() { Value = _.Id, Text = _.Name }).ToList();
            Users = userManager.Users.ToList();
        }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
