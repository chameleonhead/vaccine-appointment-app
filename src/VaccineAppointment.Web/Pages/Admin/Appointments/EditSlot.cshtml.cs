using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class EditSlotModel : PageModel
    {
        private readonly ILogger<EditSlotModel> _logger;
        private readonly AppointmentService _service;

        public LocalDate SelectedDate { get; set; }

        public AppointmentAggregate? Slot { get; set; }
        public string? ErrorMessage { get; private set; }

        [BindProperty]
        [Required]
        public string? StartTime { get; set; }

        [BindProperty]
        [Required]
        public int? DurationMinutes { get; set; }

        [BindProperty]
        [Required]
        public int? CountOfSlot { get; set; }

        public EditSlotModel(ILogger<EditSlotModel> logger, AppointmentService service)
        {
            _logger = logger;
            _service = service;
        }

        private async Task<IActionResult> PageResult(int year, int month, int day, string id)
        {
            SelectedDate = new LocalDate(year, month, day);
            Slot = await _service.FindAppointmentSlotByIdAsync(id);
            if (Slot == null)
            {
                return RedirectToPage("Index", new { year, month, day });
            }
            return Page();
        }

        public async Task<IActionResult> OnGet([FromQuery] int year, [FromQuery] int month, [FromQuery] int day, [FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("Index", new { year, month, day });
            }

            var page = await PageResult(year, month, day, id);

            StartTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Format(Slot!.From.TimeOfDay);
            DurationMinutes = (int)Slot!.Duration.ToDuration().TotalMinutes;
            CountOfSlot = Slot!.CountOfSlot;

            return page;
        }

        public async Task<IActionResult> OnPost([FromQuery] int year, [FromQuery] int month, [FromQuery] int day, [FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                return await PageResult(year, month, day, id);
            }
            var startTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Parse(StartTime!);
            if (!startTime.Success)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                return await PageResult(year, month, day, id);
            }

            var date = new LocalDate(year, month, day);
            var result = await _service.UpdateAppointmentSlotAsync(id, date.At(startTime.Value), Period.FromMinutes(DurationMinutes!.Value), CountOfSlot!.Value);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
                return await PageResult(year, month, day, id);
            }
            return RedirectToPage("SlotDetails", new { year, month, day, id });
        }
    }
}
