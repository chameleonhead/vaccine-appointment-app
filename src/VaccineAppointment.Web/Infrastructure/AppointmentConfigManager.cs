using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Infrastructure
{
    public class AppointmentConfigManager : IAppointmentConfigManager
    {
        private readonly VaccineAppointmentContext db;

        public AppointmentConfigManager(VaccineAppointmentContext db)
        {
            this.db = db;
        }

        public async Task<AppointmentConfig?> GetConfigAsync()
        {
            var config = await db.AppointmentConfig.FirstOrDefaultAsync();
            return config;
        }

        public async Task SaveConfigAsync(AppointmentConfig config)
        {
            using var tran = await db.Database.BeginTransactionAsync();
            db.AppointmentConfig.RemoveRange(db.AppointmentConfig);
            await db.AppointmentConfig.AddAsync(config);
            await db.SaveChangesAsync();
            await tran.CommitAsync();
        }
    }
}
