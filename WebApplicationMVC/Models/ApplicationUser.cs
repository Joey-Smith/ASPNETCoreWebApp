using Microsoft.AspNetCore.Identity;

namespace WebApplicationMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool Disabled { get; set; }
    }
}
