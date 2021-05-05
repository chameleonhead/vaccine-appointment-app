using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class CreateAppointmentModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;

        public LocalDate Today { get; set; }
        public YearMonth Month { get; set; }
        public LocalDate SelectedDate { get; set; }
        public YearMonth PrevMonth { get; set; }
        public YearMonth NextMonth { get; set; }

        public AppointmentsForSlot? Slot { get; set; }
        public string? ErrorMessage { get; private set; }

        [BindProperty]
        [Required]
        public string? Name { get; set; }
        [BindProperty]
        [Required]
        public string? Email { get; set; }
        [BindProperty]
        [Required]
        public string? Sex { get; set; }
        [BindProperty]
        [Required]
        public int? Age { get; set; }

        public CreateAppointmentModel(ILogger<IndexModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet([FromQuery] int year, [FromQuery] int month, [FromQuery] int day, [FromQuery] string id)
        {
            Today = TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").AtStrictly(LocalDateTime.FromDateTime(DateTime.UtcNow)).Date;
            if (!ModelState.IsValid)
            {
                return RedirectToPage("Index");
            }
            SetMonth(new YearMonth(year, month));
            SetSelectedDate(new LocalDate(year, month, day));
            Slot = await _service.FindAppointmentSlotByIdAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPost([FromQuery] int year, [FromQuery] int month, [FromQuery] int day, [FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                Today = TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").AtStrictly(LocalDateTime.FromDateTime(DateTime.UtcNow)).Date;
                if (!ModelState.IsValid)
                {
                    return RedirectToPage("Index");
                }
                SetMonth(new YearMonth(year, month));
                SetSelectedDate(new LocalDate(year, month, day));
                Slot = await _service.FindAppointmentSlotByIdAsync(id);
                return Page();
            }
            var result = await _service.CreateAppointmentAsync(id, Name!, Email!, Sex!, Age!.Value);
            return RedirectToPage("ThankYou", new { SlotId = id, Id = result.AppointmentId });
        }

        private void SetMonth(YearMonth month)
        {
            Month = month;
            PrevMonth = Month.ToDateInterval().Start.PlusDays(-1).ToYearMonth();
            NextMonth = Month.ToDateInterval().End.PlusDays(1).ToYearMonth();
        }

        private void SetSelectedDate(LocalDate date)
        {
            SelectedDate = date;
        }
    }
}
