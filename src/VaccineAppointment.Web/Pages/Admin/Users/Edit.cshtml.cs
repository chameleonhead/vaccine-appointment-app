using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VaccineAppointment.Web.Services.Users;

namespace VaccineAppointment.Web.Pages.Admin.Users
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserService _service;

        [BindProperty]
        [Required]
        public string? Username { get; set; }
        [BindProperty]
        public string? Password { get; set; }
        [BindProperty]
        [Required]
        public string? Role { get; set; }

        public EditModel(ILogger<IndexModel> logger, UserService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _service.FirstByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            Username = user.Username;
            Role = user.Role;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _service.UpdateAsync(id, Password, Role!);
            return RedirectToPage("./Index");
        }
    }
}
