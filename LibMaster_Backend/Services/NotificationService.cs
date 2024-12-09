using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LibraryManagementSystem.Services
{
    public class NotificationService
    {
        private readonly string api, _email, _name;
        public NotificationService(IConfiguration configuration)
        {
            api = configuration["SendGrid:ApiKey"];
            _email = configuration["SendGrid:SenderEmail"];
            _name = configuration["SendGrid:SenderName"];
        }

        public async Task SendNotification(string email, string name, string message)
        {
            var client = new SendGridClient(api);
            var from = new EmailAddress(_email, _name);
            var subject = "Library System Details";
            var to = new EmailAddress(email, name);
            var plainTextContent = $"Welcome {name} to Library System";
            var htmlContent = message;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
