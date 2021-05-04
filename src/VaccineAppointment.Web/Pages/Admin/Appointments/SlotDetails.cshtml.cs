using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class SlotDetailsModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;

        public LocalDate Today { get; set; }
        public YearMonth Month { get; set; }
        public LocalDate SelectedDate { get; set; }
        public YearMonth PrevMonth { get; set; }
        public YearMonth NextMonth { get; set; }

        public AppointmentSlot? Slot { get; set; }

        public SlotDetailsModel(ILogger<IndexModel> logger, AppointmentService service)
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
