using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Mailing
{
    public interface IEmailSender
    {
        Task SendMailAsync(EmailMessage param);
    }
}
