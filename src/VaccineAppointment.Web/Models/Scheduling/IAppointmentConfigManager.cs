using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public interface IAppointmentConfigManager
    {
        Task<AppointmentConfig?> GetConfigAsync();
        Task SaveConfigAsync(AppointmentConfig config);
    }
}
