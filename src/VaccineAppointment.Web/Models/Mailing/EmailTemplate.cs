using HandlebarsDotNet;
using System;

namespace VaccineAppointment.Web.Models.Mailing
{
    public class EmailTemplate
    {
        public static EmailTemplate Default => new EmailTemplate();

        public EmailTemplate()
        {
            Id = Guid.Empty.ToString();
            TemplateName = "Unknown";
        }

        public string Id { get; set; }
        public string TemplateName { get; set; }
        public string? FromTemplate { get; set; }
        public string? ToTemplate { get; set; }
        public string? CcTemplate { get; set; }
        public string? BccTemplate { get; set; }
        public string? SubjectTemplate { get; set; }
        public string? BodyTemplate { get; set; }


        public EmailMessage CreateMessage(EmailMessageParams param)
        {
            return new EmailMessage()
            {
                From = param.From ?? RenderText(FromTemplate, param) ?? throw new ArgumentNullException(nameof(param.From)),
                To = param.To ?? RenderText(ToTemplate, param) ?? throw new ArgumentNullException(nameof(param.To)),
                Cc = param.Cc ?? RenderText(CcTemplate, param),
                Bcc = param.Bcc ?? RenderText(BccTemplate, param),
                Subject = param.Subject ?? RenderText(SubjectTemplate, param) ?? throw new ArgumentNullException(),
                Body = param.Body ?? RenderText(BodyTemplate, param) ?? throw new ArgumentNullException(),
            };
        }

        private string? RenderText(string? source, EmailMessageParams param)
        {
            if (source is null)
            {
                return null;
            }
            var handlebars = Handlebars.Create();
            handlebars.Configuration.NoEscape = true;
            var template = handlebars.Compile(source);
            return template(param.Params);
        }
    }
}
