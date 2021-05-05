using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Scheduling;

namespace VaccineAppointment.Web.Pages
{

    public class ThankYouModel : PageModel
    {
        private readonly ILogger<ThankYouModel> _logger;
        private readonly AppointmentService _service;

        public Appointment? Appointment { get; set; }


        public ThankYouModel(ILogger<ThankYouModel> logger, AppointmentService service)
        {
            _service = service;
            _logger = logger;
        }

        public async Task OnGet([FromQuery] string id)
        {
            var slot = await _service.FindAppointmentByIdAsync(id);
            Appointment = slot!.FindAppointmentById(id);
        }
    }
}
