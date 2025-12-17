// demoGrilla6/Services/SmtpEmailSender.cs
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace demoGrilla6.Services
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;

        public SmtpEmailSender(SmtpSettings settings)
        {
            _settings = settings;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            using var message = new MailMessage(_settings.From, to, subject, body)
            {
                IsBodyHtml = false
            };

            await client.SendMailAsync(message);
        }
    }

    public sealed class SmtpSettings
    {
        public string Host { get; init; } = string.Empty;
        public int Port { get; init; } = 587;
        public bool EnableSsl { get; init; } = true;
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string From { get; init; } = string.Empty;
    }
}
