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
    public class CreateModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserService _service;

        [BindProperty]
        [Required]
        public string? Name { get; set; }
        [BindProperty]
        [Required]
        public string? Username { get; set; }
        [BindProperty]
        [Required]
        public string? Password { get; set; }
        [BindProperty]
        [Required]
        public string? Role { get; set; }

        public CreateModel(ILogger<IndexModel> logger, UserService service)
        {
            _logger = logger;
            _service = service;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _service.CreateAsync(Username!, Password!, Role!, Name!);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
