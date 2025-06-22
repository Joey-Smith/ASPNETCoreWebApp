using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApplicationMVC.Areas.Template.Configurations
{
    public static class ApplicationConfiguration
    {
        public static IConfiguration config;
        public static void Initialize(IConfiguration Configuration)
        {
            config = Configuration;
        }

        public static string Name
        {
            get { return config.GetSection("Name").Value; }
        }

        public static bool AllowRegistration
        {
            get { return bool.Parse(config.GetSection("AllowRegistration").Value); }
        }

        public static bool AllowLogin
        {
            get { return bool.Parse(config.GetSection("AllowLogin").Value); }
        }
    }
}
