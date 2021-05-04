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
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class EditSlotModel : PageModel
    {
        private readonly AppointmentService _service;
        private readonly ILogger<IndexModel> _logger;

        public LocalDate Today { get; set; }
        public YearMonth Month { get; set; }
        public LocalDate SelectedDate { get; set; }
        public YearMonth PrevMonth { get; set; }
        public YearMonth NextMonth { get; set; }

        public AppointmentSlot? Slot { get; set; }
        public string? ErrorMessage { get; private set; }

        [BindProperty]
        [Required]
        public string? StartTime { get; set; }

        [BindProperty]
        [Required]
        public string? EndTime { get; set; }

        [BindProperty]
        [Required]
        public int? CountOfSlot { get; set; }

        public EditSlotModel(ILogger<IndexModel> logger, AppointmentService service)
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
            if (Slot == null)
            {
                return RedirectToPage("Index");
            }

            StartTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Format(Slot!.From.TimeOfDay);
            EndTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Format(Slot!.To.TimeOfDay);
            CountOfSlot = Slot!.CountOfSlot;

            return Page();
        }

        public async Task<IActionResult> OnPost([FromQuery] int year, [FromQuery] int month, [FromQuery] int day, [FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                SetMonth(new YearMonth(year, month));
                SetSelectedDate(new LocalDate(year, month, day));
                Slot = await _service.FindAppointmentSlotByIdAsync(id);
                return Page();
            }
            var startTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Parse(StartTime!);
            var endTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Parse(EndTime!);
            if (!startTime.Success || !endTime.Success)
            {
                ErrorMessage = "ì¸óÕÇ…åÎÇËÇ™Ç†ÇËÇ‹Ç∑ÅB";
                SetMonth(new YearMonth(year, month));
                SetSelectedDate(new LocalDate(year, month, day));
                Slot = await _service.FindAppointmentSlotByIdAsync(id);
                return Page();
            }

            var date = new LocalDate(year, month, day);
            var result = await _service.UpdateAppointmentSlotAsync(id, date.At(startTime.Value), date.At(endTime.Value), CountOfSlot!.Value);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
                SetMonth(new YearMonth(year, month));
                SetSelectedDate(new LocalDate(year, month, day));
                Slot = await _service.FindAppointmentSlotByIdAsync(id);
                return Page();
            }
            return RedirectToPage("SlotDetails", new { year, month, day, id });
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
