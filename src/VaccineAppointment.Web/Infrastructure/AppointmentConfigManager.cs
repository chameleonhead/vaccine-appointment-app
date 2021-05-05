using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Infrastructure
{
    public class AppointmentConfigManager : IAppointmentConfigManager
    {
        private VaccineAppointmentContext vaccineAppointmentContext;

        public AppointmentConfigManager(VaccineAppointmentContext vaccineAppointmentContext)
        {
            this.vaccineAppointmentContext = vaccineAppointmentContext;
        }

        public Task<AppointmentConfig> GetConfigAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task SaveConfigAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
