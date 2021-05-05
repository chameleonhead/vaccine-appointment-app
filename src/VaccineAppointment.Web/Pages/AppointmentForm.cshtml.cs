using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages
{

    public class AppointmentFormModel : PageModel
    {
        private readonly ILogger<AppointmentFormModel> _logger;
        private readonly AppointmentService _service;

        public AppointmentAggregate? AppointmentSlot { get; set; }
        public string? ErrorMessage { get; set; }

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

        public AppointmentFormModel(ILogger<AppointmentFormModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        public async Task OnGet([FromQuery] string id)
        {
            AppointmentSlot = await _service.FindAppointmentSlotByIdAsync(id);
        }

        public async Task<IActionResult> OnPost([FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "入力に誤りがあります。";
                AppointmentSlot = await _service.FindAppointmentSlotByIdAsync(id);
                return Page();
            }
            var result = await _service.MakeAppointmentAsync(id, Name!, Email!, Sex!, Age!.Value);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
                AppointmentSlot = await _service.FindAppointmentSlotByIdAsync(id);
                return Page();
            }
            var appointmentId = (result as MakeAppointmentResult)!.AppointmentId;
            return RedirectToPage("ThankYou", new { SlotId = id, Id = appointmentId });
        }
    }
}
