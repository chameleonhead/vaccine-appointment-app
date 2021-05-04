using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using NodaTime.TimeZones;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Tasks;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class CreateSlotModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;

        public LocalDate Today { get; set; }
        public YearMonth Month { get; set; }
        public LocalDate SelectedDate { get; set; }
        public YearMonth PrevMonth { get; set; }
        public YearMonth NextMonth { get; set; }

        public string? ErrorMessage { get; set; }

        [BindProperty]
        [Required]
        public string? StartTime { get; set; }

        [BindProperty]
        [Required]
        public string? EndTime { get; set; }

        [BindProperty]
        [Required]
        public int? CountOfSlot { get; set; }

        public CreateSlotModel(ILogger<IndexModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        public void OnGet([FromQuery] int? year, [FromQuery] int? month, [FromQuery] int? day)
        {
            Today = TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo").AtStrictly(LocalDateTime.FromDateTime(DateTime.UtcNow)).Date;
            if (year.HasValue && month.HasValue)
            {
                SetMonth(new YearMonth(year.Value, month.Value));
            }
            else
            {
                SetMonth(Today.ToYearMonth());
            }
            if (day.HasValue)
            {
                SetSelectedDate(Month.OnDayOfMonth(day.Value));
            }
            else
            {
                SetSelectedDate(Today);
            }
            StartTime = "09:00";
            EndTime = "09:30";
            CountOfSlot = 1;
        }

        public async Task<IActionResult> OnPost([FromQuery] int? year, [FromQuery] int? month, [FromQuery] int? day)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                return Page();
            }
            var startTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Parse(StartTime!);
            var endTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Parse(EndTime!);
            if (!startTime.Success || !endTime.Success)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                return Page();
            }
            var result = await _service.CreateAppointmentSlotAsync(startTime.Value, endTime.Value, CountOfSlot!.Value);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
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
