using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Mailing
{
    public interface IEmailConfigurationManager
    {
        Task<EmailConfiguration?> GetConfigurationAsync();
    }
}
