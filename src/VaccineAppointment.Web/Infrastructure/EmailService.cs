using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Infrastructure
{
    public class EmailConfiguration
    {
        public string? Host { get; set; }
        public int? Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Sender { get; set; }
        public string? Signature { get; set; }
    }

    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration configuration;

        public EmailService(EmailConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendMailAsync(string to, string title, string body)
        {
            if (configuration == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(configuration.Host) || !configuration.Port.HasValue)
            {
                return;
            }
            if (string.IsNullOrEmpty(configuration.Sender))
            {
                return;
            }
            using (var client = new SmtpClient(configuration.Host, configuration.Port!.Value))
            {
                client.EnableSsl = true;
                if (!string.IsNullOrEmpty(configuration.Username))
                {
                    client.Credentials = new NetworkCredential(configuration.Username, configuration.Password);
                }
                var mailMessage = new MailMessage(new MailAddress(configuration.Sender!), new MailAddress(to))
                {
                    Subject = title,
                    Body = body
                };
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
