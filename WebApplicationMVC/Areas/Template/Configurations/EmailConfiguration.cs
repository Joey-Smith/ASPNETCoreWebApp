using MailKit.Security;

namespace WebApplicationMVC.Areas.Template.Configurations
{
    public class EmailConfiguration
    {
        public EmailConfiguration()
        {
            Smtp = "";
            From = "";
            Password = "";
            SecureSocketOptions = SecureSocketOptions.Auto;
        }
        public string Smtp { get; set; }
        public string From { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public SecureSocketOptions SecureSocketOptions { get; set; }
    }
}
