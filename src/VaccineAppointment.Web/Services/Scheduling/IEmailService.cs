using System.Threading.Tasks;

namespace VaccineAppointment.Web.Services.Scheduling
{
    public interface IEmailService
    {
        Task SendMailAsync(string to, string title, string body);
    }
}
