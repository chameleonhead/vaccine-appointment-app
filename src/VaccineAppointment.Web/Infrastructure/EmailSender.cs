using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Mailing;

namespace VaccineAppointment.Web.Infrastructure
{

    public class EmailSender : IEmailSender
    {
        private readonly IEmailConfigurationManager _configurationManager;

        public EmailSender(IEmailConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public async Task SendMailAsync(EmailMessage message)
        {
            var configuration = await _configurationManager.GetConfigurationAsync();
            if (configuration == null)
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
                await client.SendMailAsync(ConvertMessage(message));
            }
        }

        private static MailMessage ConvertMessage(EmailMessage message)
        {
            var mailMessage = new MailMessage(new MailAddress(message.From), new MailAddress(message.To))
            {
                Subject = message.Subject,
                Body = message.Body,
            };
            if (!string.IsNullOrEmpty(message.Cc))
            {
                foreach (var cc in message.Cc.Split(";"))
                {
                    mailMessage.CC.Add(cc);
                }
            }
            if (!string.IsNullOrEmpty(message.Bcc))
            {
                foreach (var bcc in message.Bcc.Split(";"))
                {
                    mailMessage.Bcc.Add(bcc);
                }
            }
            return mailMessage;
        }
    }
}
