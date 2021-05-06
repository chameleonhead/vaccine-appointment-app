using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Mailing
{
    public interface IEmailTemplateRepository
    {
        Task<EmailTemplate> FindByNameAsync(string templateName);
    }
}
