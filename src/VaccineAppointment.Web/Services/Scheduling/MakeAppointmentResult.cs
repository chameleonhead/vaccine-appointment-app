namespace VaccineAppointment.Web.Services.Scheduling
{
    public class MakeAppointmentResult : OperationResult
    {
        public string? AppointmentId { get; }

        public MakeAppointmentResult(string appointmentId, string? errorMessage = null) : base(errorMessage)
        {
            AppointmentId = appointmentId;
        }

        public static MakeAppointmentResult Ok(string appointmentId)
        {
            return new MakeAppointmentResult(appointmentId);
        }
    }
}