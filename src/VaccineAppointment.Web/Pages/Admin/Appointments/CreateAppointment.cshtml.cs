using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages.Admin.Appointments
{
    [Authorize]
    public class CreateAppointmentModel : PageModel
    {
        private readonly ILogger<CreateAppointmentModel> _logger;
        private readonly AppointmentService _service;

        public LocalDate SelectedDate { get; set; }
        public AppointmentAggregate? Slot { get; set; }
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

        public CreateAppointmentModel(ILogger<CreateAppointmentModel> logger, AppointmentService service)
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
                return RedirectToPage("Index");
            }
            return await PageResult(year, month, day, id);
        }

        public async Task<IActionResult> OnPost([FromQuery] int year, [FromQuery] int month, [FromQuery] int day, [FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "?????????????????????B";
                return await PageResult(year, month, day, id);
            }
            var result = await _service.CreateAppointmentAsync(id, Name!, Email!, Sex!, Age!.Value);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
                return await PageResult(year, month, day, id);
            }
            return RedirectToPage("SlotDetails", new { year, month, day, id });
        }
    }
}
