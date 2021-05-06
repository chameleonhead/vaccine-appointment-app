namespace VaccineAppointment.Web.Models.Mailing
{
    public class EmailMessageParams
    {
        public string? TemplateName { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Cc { get; set; }
        public string? Bcc { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public object? Params { get; set; }
    }
}