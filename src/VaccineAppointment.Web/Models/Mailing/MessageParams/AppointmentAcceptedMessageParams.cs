using NodaTime;

namespace VaccineAppointment.Web.Models.Mailing.MessageParams
{
    public class AppointmentAcceptedMessageParams
    {
        public AppointmentAcceptedMessageParams(string appointmentId, LocalDate date, LocalTime fromTime, LocalTime toTime, string name)
        {
            AppointmentId = appointmentId;
            Date = date;
            FromTime = fromTime;
            ToTime = toTime;
            Name = name;
        }

        public string AppointmentId { get; set; }
        public LocalDate Date { get; set; }
        public LocalTime FromTime { get; set; }
        public LocalTime ToTime { get; set; }
        public string Name { get; set; }
    }
}
