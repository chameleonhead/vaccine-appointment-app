namespace VaccineAppointment.Web.Models.Mailing
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            From = "NOTSET";
            To = "NOTSET";
            Subject = "NOTSET";
            Body = "NOTSET";
        }

        public string From { get; set; }
        public string To { get; set; }
        public string? Cc { get; set; }
        public string? Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
