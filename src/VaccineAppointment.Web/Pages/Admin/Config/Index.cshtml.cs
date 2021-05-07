using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Config
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IAppointmentConfigManager _appointmentConfigManager;

        [BindProperty]
        public string? AvailableIntervalStart { get; set; }
        [BindProperty]
        public string? AvailableIntervalEnd { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IAppointmentConfigManager appointmentConfigManager)
        {
            _logger = logger;
            _appointmentConfigManager = appointmentConfigManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var appointmentConfig = await _appointmentConfigManager.GetConfigAsync();
            AvailableIntervalStart = FormatDate(appointmentConfig.AvailableIntervalStart);
            AvailableIntervalEnd = FormatDate(appointmentConfig.AvailableIntervalEnd);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var appointmentConfig = new AppointmentConfig();
            appointmentConfig.AvailableIntervalStart = ParseDate(AvailableIntervalStart);
            appointmentConfig.AvailableIntervalEnd = ParseDate(AvailableIntervalEnd);
            await _appointmentConfigManager.SaveConfigAsync(appointmentConfig);
            return RedirectToPage();
        }

        private static LocalDate? ParseDate(string? localDateString)
        {
            if (string.IsNullOrEmpty(localDateString))
            {
                return null;
            }
            var result = LocalDatePattern.Iso.Parse(localDateString);
            if (result.Success)
            {
                return result.Value;
            }
            return null;
        }

        private static string? FormatDate(LocalDate? localDate)
        {
            if (localDate.HasValue)
            {
                return LocalDatePattern.Iso.Format(localDate.Value);
            }
            return null;
        }
    }
}
