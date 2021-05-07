using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Mailing
{
    public interface IEmailConfigManager
    {
        Task<EmailConfig> GetConfigAsync();
        Task SaveConfigAsync(EmailConfig config);
    }
}
