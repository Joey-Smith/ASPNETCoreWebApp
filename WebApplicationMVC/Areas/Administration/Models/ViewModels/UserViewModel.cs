using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplicationMVC.Areas.Administration.Models.ViewModels
{
    public class UserViewModel
    {
        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }
        public bool Disabled { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }
}
