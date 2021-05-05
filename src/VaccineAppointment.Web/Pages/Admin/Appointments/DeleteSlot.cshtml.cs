using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Threading.Tasks;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class DeleteSlotModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;

        public LocalDate Today { get; set; }
        public YearMonth Month { get; set; }
        public LocalDate SelectedDate { get; set; }
        public YearMonth PrevMonth { get; set; }
        public YearMonth NextMonth { get; set; }
        public string? ErrorMessage { get; set; }

        public AppointmentsForSlot? Slot { get; set; }

        public DeleteSlotModel(ILogger<IndexModel> logger, AppointmentService service)
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
            var result = await _service.DeleteAppointmentSlotAsync(id);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
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
            return RedirectToPage("Index", new { year, month, day });
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
