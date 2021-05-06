using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Mailing;

namespace VaccineAppointment.Web.Infrastructure
{
    public class EmailConfigurationManager : IEmailConfigurationManager
    {
        public IConfiguration Configuration { get; }

        public EmailConfigurationManager(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Task<EmailConfiguration?> GetConfigurationAsync()
        {
            var configuration = Configuration.GetSection("VaccineAppointment.Web:Email").Get<EmailConfiguration>();
            if (configuration != null)
            {
                if (string.IsNullOrEmpty(configuration.Host) || !configuration.Port.HasValue)
                {
                    configuration = null;
                }
            }
            return Task.FromResult(configuration);
        }
    }
}
