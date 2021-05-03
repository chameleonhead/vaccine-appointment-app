using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
                ErrorMessage = "入力に誤りがあります。";
                return Page();
            }

            if (NewPassword != NewPasswordConfirm)
            {
                ErrorMessage = "新しパスワードが一致しません。";
                return Page();
            }

            var user = await _service.FindByUsernameAsync(Username!);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var result = await _service.ChangePasswordAsync(Username, _passwordHasher.Hash(NewPassword!));
            if (!result.Succeeded)
            {
                ErrorMessage = result.ErrorMessage;
                return Page();
            }

            InfoMessage = "パスワードを変更しました。";
            return RedirectToPage("");
        }
    }
}
