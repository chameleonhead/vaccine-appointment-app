using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public string? Password { get; set; }

        [BindProperty]
        [Required]
        public string? NewPassword { get; set; }

        [BindProperty]
        [Required]
        public string? NewPasswordConfirm { get; set; }

        public string? ErrorMessage { get; set; }
        public string? InfoMessage { get; private set; }

        public ChangePasswordModel(UserService service, PasswordHasher passwordHasher)
        {
            _service = service;
            _passwordHasher = passwordHasher;
        }

        public void OnGet()
        {
            Username = HttpContext.User.Identity?.Name ?? throw new InvalidOperationException("Identity is null");
        }

        public async Task<IActionResult> OnPost()
        {
            Username = HttpContext.User.Identity?.Name ?? throw new InvalidOperationException("Identity is null");
            if (!ModelState.IsValid)
            {
                ErrorMessage = "���͂Ɍ�肪����܂��B";
                return Page();
            }

            if (NewPassword != NewPasswordConfirm)
            {
                ErrorMessage = "�V���p�X���[�h����v���܂���B";
                return Page();
            }

            var user = await _service.FindByUsernameAsync(Username!);
            if (user == null || user.Password != _passwordHasher.Hash(Password!))
            {
                ErrorMessage = "���݂̃p�X���[�h����v���܂���B";
                return Page();
            }

            var result = await _service.ChangePasswordAsync(Username, NewPassword!);
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
                return Page();
            }

            InfoMessage = "�p�X���[�h��ύX���܂����B";
            return RedirectToPage("");
        }
    }
}
