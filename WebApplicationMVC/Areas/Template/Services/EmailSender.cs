using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using WebApplicationMVC.Areas.Template.Configurations;

namespace WebApplicationMVC.Areas.Template.Services
{
    public class EmailSender : IEmailSender
    {
        private EmailConfiguration _emailConfiguration;

        public EmailSender(IOptions<EmailConfiguration> emailConfiguration)
        {
            _emailConfiguration = emailConfiguration.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string response;
            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();
            var client = new SmtpClient();

            message.From.Add(new MailboxAddress(null, _emailConfiguration.From));
            message.To.Add(new MailboxAddress(null, email));
            message.Subject = subject;

            bodyBuilder.HtmlBody = htmlMessage;

            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                client.Connect(_emailConfiguration.Smtp, _emailConfiguration.Port, _emailConfiguration.SecureSocketOptions);
                client.Authenticate(_emailConfiguration.From, _emailConfiguration.Password);
                response = client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                if (client.IsConnected) client.Disconnect(true);
                return Task.FromException(ex);
            }

            return Task.FromResult(0);
        }
    }
}
