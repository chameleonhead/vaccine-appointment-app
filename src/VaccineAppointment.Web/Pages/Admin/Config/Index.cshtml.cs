using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Mailing;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Services.Mailing;

namespace VaccineAppointment.Web.Pages.Admin.Config
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IAppointmentConfigManager _appointmentConfigManager;
        private readonly IEmailConfigManager _emailConfigurationManager;
        private readonly EmailService _service;

        public string? ErrorMessage { get; private set; }

        [BindProperty]
        public string? AvailableIntervalStart { get; set; }
        [BindProperty]
        public string? AvailableIntervalEnd { get; set; }

        [BindProperty]
        public string? Host { get; set; }
        [BindProperty]
        public int? Port { get; set; }
        [BindProperty]
        public bool UseSsl { get; set; }
        [BindProperty]
        public string? Username { get; set; }
        [BindProperty]
        public string? Password { get; set; }
        [BindProperty]
        public string? ToAddress { get; set; }

        [BindProperty]
        public string? AppointmentAcceptedMessageFromTemplate { get; set; }

        [BindProperty]
        public string? AppointmentAcceptedMessageSubjectTemplate { get; set; }

        [BindProperty]
        [DataType(DataType.MultilineText)]
        public string? AppointmentAcceptedMessageBodyTemplate { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IAppointmentConfigManager appointmentConfigManager, IEmailConfigManager emailConfigurationManager, EmailService service)
        {
            _logger = logger;
            _appointmentConfigManager = appointmentConfigManager;
            _emailConfigurationManager = emailConfigurationManager;
            _service = service;
        }

        public async Task<IActionResult> OnGet()
        {
            var appointmentConfig = await _appointmentConfigManager.GetConfigAsync();
            AvailableIntervalStart = FormatDate(appointmentConfig.AvailableIntervalStart);
            AvailableIntervalEnd = FormatDate(appointmentConfig.AvailableIntervalEnd);
            var emailConfig = await _emailConfigurationManager.GetConfigAsync();
            Host = emailConfig.Host;
            Port = emailConfig.Port;
            UseSsl = emailConfig.UseSsl;
            Username = emailConfig.Username;
            Password = emailConfig.Password;
            var messageTemplate = await _service.FindTempalateByNameAsync("AppointmentAcceptedMessage");
            AppointmentAcceptedMessageFromTemplate = messageTemplate?.FromTemplate;
            AppointmentAcceptedMessageSubjectTemplate = messageTemplate?.SubjectTemplate;
            AppointmentAcceptedMessageBodyTemplate = messageTemplate?.BodyTemplate;
            return Page();
        }

        public async Task<IActionResult> OnPostAppointmentConfig()
        {
            var appointmentConfig = new AppointmentConfig();
            appointmentConfig.AvailableIntervalStart = ParseDate(AvailableIntervalStart);
            appointmentConfig.AvailableIntervalEnd = ParseDate(AvailableIntervalEnd);
            await _appointmentConfigManager.SaveConfigAsync(appointmentConfig);
            return RedirectToPage();
        }

        private static LocalDate? ParseDate(string? localDateString)
        {
            if (string.IsNullOrEmpty(localDateString))
            {
                return null;
            }
            var result = LocalDatePattern.Iso.Parse(localDateString);
            if (result.Success)
            {
                return result.Value;
            }
            return null;
        }

        private static string? FormatDate(LocalDate? localDate)
        {
            if (localDate.HasValue)
            {
                return LocalDatePattern.Iso.Format(localDate.Value);
            }
            return null;
        }

        public async Task<IActionResult> OnPostEmailConfig()
        {
            if (!string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "パスワードが入力されていません。";
                return Page();
            }
            var emailConfig = new EmailConfig();
            emailConfig.Host = Host;
            emailConfig.Port = Port;
            emailConfig.UseSsl = UseSsl;
            emailConfig.Username = Username;
            emailConfig.Password = Password;
            await _emailConfigurationManager.SaveConfigAsync(emailConfig);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostTestSendingEmail()
        {
            if (string.IsNullOrEmpty(Host) || !Port.HasValue)
            {
                ErrorMessage = "設定値が入力されていません。";
                return Page();
            }
            if (!string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "パスワードが入力されていません。";
                return Page();
            }
            if (string.IsNullOrEmpty(ToAddress))
            {
                ErrorMessage = "送信先を指定してください。";
                return Page();
            }
            var emailConfig = new EmailConfig();
            emailConfig.Host = Host;
            emailConfig.Port = Port;
            emailConfig.UseSsl = UseSsl;
            emailConfig.Username = Username;
            emailConfig.Password = Password;
            await _emailConfigurationManager.SaveConfigAsync(emailConfig);

            await _service.SendMailAsync(new EmailMessageParams()
            {
                From = ToAddress,
                To = ToAddress,
                Subject = "Test",
                Body = "This is test mail.",
            });
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAppointmentAcceptedMessageEmailTemplate()
        {
            if (string.IsNullOrEmpty(AppointmentAcceptedMessageFromTemplate))
            {
                ErrorMessage = "差出人が入力されていません。";
                return Page();
            }
            if (string.IsNullOrEmpty(AppointmentAcceptedMessageSubjectTemplate))
            {
                ErrorMessage = "件名が入力されていません。";
                return Page();
            }
            if (string.IsNullOrEmpty(AppointmentAcceptedMessageBodyTemplate))
            {
                ErrorMessage = "本文が入力されていません。";
                return Page();
            }

            await _service.SaveTemplateAsync(new EmailTemplate()
            {
                TemplateName = "AppointmentAcceptedMessage",
                FromTemplate = AppointmentAcceptedMessageFromTemplate,
                SubjectTemplate = AppointmentAcceptedMessageSubjectTemplate,
                BodyTemplate = AppointmentAcceptedMessageBodyTemplate,
            });
            return RedirectToPage();
        }
    }
}
