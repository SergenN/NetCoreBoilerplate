using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NetCoreBoilerplate.Models.Settings;

namespace NetCoreBoilerplate.Services.Messaging
{
    public class MessageService : IEmailSender, ISmsSender
    {
        private IOptions<MailSettings> _settings;

        public MessageService(IOptions<MailSettings> settings)
        {
            _settings = settings;
        }
        
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_settings.Value.Domain, 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.Value.UserName, _settings.Value.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.Value.FromEmail, _settings.Value.FromName),
                To = { new MailAddress(email) },
                Body = htmlMessage,
                Subject = subject
            };
            
            await client.SendMailAsync(mailMessage);
        }

        public Task SendSmsAsync(string number, string message)
        {
            throw new NotImplementedException();
        }
    }
}