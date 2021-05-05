using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Tasks;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class CreateSlotModel : PageModel
    {
        private readonly ILogger<CreateSlotModel> _logger;
        private readonly AppointmentService _service;

        public LocalDate SelectedDate { get; set; }

        public string? ErrorMessage { get; set; }

        [BindProperty]
        [Required]
        public string? StartTime { get; set; }

        [BindProperty]
        [Required]
        public int? DurationMinutes { get; set; }

        [BindProperty]
        [Required]
        public int? CountOfSlot { get; set; }

        public CreateSlotModel(ILogger<CreateSlotModel> logger, AppointmentService service)
        {
            _logger = logger;
            _service = service;
        }

        private IActionResult PageResult(int year, int month, int day)
        {
            SelectedDate = new LocalDate(year, month, day);
            return Page();
        }

        public IActionResult OnGet([FromQuery] int year, [FromQuery] int month, [FromQuery] int day)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("Index", new { year, month, day });
            }

            StartTime = "09:00";
            DurationMinutes = 30;
            CountOfSlot = 1;

            return PageResult(year, month, day);
        }

        public async Task<IActionResult> OnPost([FromQuery] int year, [FromQuery] int month, [FromQuery] int day)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                return PageResult(year, month, day);
            }
            var startTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Parse(StartTime!);
            if (!startTime.Success)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                return PageResult(year, month, day);
            }
            var date = new LocalDate(year, month, day);
            var result = await _service.CreateAppointmentSlotAsync(date.At(startTime.Value), Period.FromMinutes(DurationMinutes!.Value), CountOfSlot!.Value);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
                return PageResult(year, month, day);
            }
            return RedirectToPage("Index", new { year, month, day });
        }
    }
}
