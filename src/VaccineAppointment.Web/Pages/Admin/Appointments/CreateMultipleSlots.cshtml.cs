using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
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
        private readonly ILogger<CreateMultipleSlotsModel> _logger;

        public LocalDate SelectedDate { get; set; }

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

        public CreateMultipleSlotsModel(ILogger<CreateMultipleSlotsModel> logger, AppointmentService service)
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
            DurationMinutesForEachSlot = 30;
            CountOfSlotForEachSlot = 1;
            CountOfSlotsToCreate = 1;
            if (!SelectedDates.Any())
            {
                SelectedDates.Add(LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd").Format(SelectedDate));
            }

            return PageResult(year, month, day);
        }

        public async Task<IActionResult> OnPost([FromQuery] int year, [FromQuery] int month, [FromQuery] int day)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "入力に誤りがあります。";
                return PageResult(year, month, day);
            }
            if (!SelectedDates.Any())
            {
                ErrorMessage = "対象の日を1日以上は設定してください。";
                return PageResult(year, month, day);
            }
            var startTime = LocalTimePattern.Create("HH:mm", CultureInfo.CurrentCulture).Parse(StartTime!);
            if (!startTime.Success)
            {
                ErrorMessage = "入力に誤りがあります。";
                return PageResult(year, month, day);
            }

            var dates = SelectedDates
                .Select(str => LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd").Parse(str))
                .ToList();
            if (dates.Any(r => !r.Success))
            {
                ErrorMessage = "入力に誤りがあります。";
                return PageResult(year, month, day);
            }
            var result = await _service.CreateMultipleAppointmentSlotsAsync(dates.Select(d => d.Value).OrderBy(d => d).ToList(), startTime.Value, Period.FromMinutes(DurationMinutesForEachSlot!.Value), CountOfSlotForEachSlot!.Value, CountOfSlotsToCreate!.Value);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
                return PageResult(year, month, day);
            }

            return RedirectToPage("Index", new { year, month, day });
        }
    }
}
