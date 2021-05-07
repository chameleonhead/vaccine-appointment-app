using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VaccineAppointment.Web.Infrastructure.Models;
using VaccineAppointment.Web.Models.Mailing;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Infrastructure
{
    public class ConfigurationManager : IEmailConfigManager, IAppointmentConfigManager
    {
        private readonly VaccineAppointmentContext db;

        public ConfigurationManager(VaccineAppointmentContext db)
        {
            this.db = db;
        }

        public async Task<T> GetConfigAsync<T>() where T : new()
        {
            var config = await db.Configurations.FirstOrDefaultAsync(d => d.Name == typeof(T).FullName);
            if (config == null)
            {
                return new T();
            }
            return config.Get<T>() ?? new T();
        }

        public async Task SaveConfigAsync<T>(T config) where T : new()
        {
            var configuration = db.Configurations.Where(c => c.Name == typeof(T).FullName).FirstOrDefault();
            using var tran = await db.Database.BeginTransactionAsync();
            if (configuration != null)
            {
                configuration.Set(config);
                db.Configurations.Update(configuration);
            }
            else
            {
                await db.Configurations.AddAsync(Configuration.From(config));
            }
            await db.SaveChangesAsync();
            await tran.CommitAsync();
        }

        public Task SaveConfigAsync(EmailConfig config)
        {
            return SaveConfigAsync<EmailConfig>(config);
        }

        Task<EmailConfig> IEmailConfigManager.GetConfigAsync()
        {
            return GetConfigAsync<EmailConfig>();
        }

        public Task SaveConfigAsync(AppointmentConfig config)
        {
            return SaveConfigAsync<AppointmentConfig>(config);
        }

        Task<AppointmentConfig> IAppointmentConfigManager.GetConfigAsync()
        {
            return GetConfigAsync<AppointmentConfig>();
        }
    }
}
