using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class CreateMultipleSlotsModel : PageModel
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
        public List<string> SelectedDates { get; } = new List<string>();

        [BindProperty]
        [Required]
        public string? StartTime { get; set; }

        [BindProperty]
        [Required]
        public int? DurationMinutesForEachSlot { get; set; }

        [BindProperty]
        [Required]
        public int? CountOfSlotForEachSlot { get; set; }

        [BindProperty]
        [Required]
        public int? CountOfSlotsToCreate { get; set; }

        public CreateMultipleSlotsModel(ILogger<IndexModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        public void OnGet([FromQuery] int year, [FromQuery] int month, [FromQuery] int day)
        {
            PrepareForShow(year, month, day);
        }

        public async Task<IActionResult> OnPost([FromQuery] int year, [FromQuery] int month, [FromQuery] int day)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                PrepareForShow(year, month, day);
                return Page();
            }
            if (!SelectedDates.Any())
            {
                ErrorMessage = "ëŒè€ÇÃì˙Ç1ì˙à»è„ÇÕê›íËÇµÇƒÇ≠ÇæÇ≥Ç¢ÅB";
                PrepareForShow(year, month, day);
                return Page();
            }
            var startTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Parse(StartTime!);
            if (!startTime.Success)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                PrepareForShow(year, month, day);
                return Page();
            }
            var date = new LocalDate(year, month, day);
            var result = await _service.CreateMultipleAppointmentSlotsAsync(date.At(startTime.Value), Period.FromMinutes(DurationMinutesForEachSlot!.Value), CountOfSlotForEachSlot!.Value, CountOfSlotsToCreate!.Value);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
                return Page();
            }
            return RedirectToPage("Index", new { year, month, day });
        }

        private void PrepareForShow(int? year, int? month, int? day)
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
            DurationMinutesForEachSlot = 30;
            CountOfSlotForEachSlot = 1;
            CountOfSlotsToCreate = 1;
            if (!SelectedDates.Any())
            {
                SelectedDates.Add(LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd").Format(SelectedDate));
            }
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
