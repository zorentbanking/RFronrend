using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace Zorent.Common.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual async Task SendResetEmailAsync(
            string toEmail,
            string resetLink)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(
                "zorentbanking@gmail.com",
                "Zorent Banking");

            var to = new EmailAddress(toEmail);

            var subject = "Reset Password";

            var htmlContent = $@"
                <h2>Password Reset</h2>

                <p>
                    Click below to reset your password:
                </p>

                <a href='{resetLink}'>
                    Reset Password
                </a>

                <p>
                    This link expires in 15 minutes.
                </p>
            ";

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                "",
                htmlContent
            );

            await client.SendEmailAsync(msg);
        }
    }
}