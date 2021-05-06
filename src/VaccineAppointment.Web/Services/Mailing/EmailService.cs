using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Mailing;

namespace VaccineAppointment.Web.Services.Mailing
{
    public class EmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateRepository _templateRepository;

        public EmailService(IEmailSender emailSender, IEmailTemplateRepository templateRepository)
        {
            _emailSender = emailSender;
            _templateRepository = templateRepository;
        }

        public async Task SendMailAsync(EmailMessageParams param)
        {
            EmailTemplate messageTemplate;
            if (string.IsNullOrEmpty(param.TemplateName))
            {
                messageTemplate = EmailTemplate.Default;
            }
            else
            {
                messageTemplate = await _templateRepository.FindByNameAsync(param.TemplateName);
            }
            await _emailSender.SendMailAsync(messageTemplate.CreateMessage(param));
        }
    }
}
