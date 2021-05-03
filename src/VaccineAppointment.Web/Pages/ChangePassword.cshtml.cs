using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VaccineAppointment.Web.Authentication;
using VaccineAppointment.Web.Services.Users;

namespace VaccineAppointment.Web.Pages.Admin
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserService _service;
        private readonly PasswordHasher _passwordHasher;

        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

        public ChangePasswordModel(UserService service, PasswordHasher passwordHasher)
        {
            _service = service;
            _passwordHasher = passwordHasher;
        }

        public void OnGet()
        {
            Username = HttpContext.User.Identity?.Name ?? throw new InvalidOperationException("Identity is null");
        }

        public void OnPost()
        {

        }
    }
}
