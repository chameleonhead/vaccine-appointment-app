namespace VaccineAppointment.Web.Services.Scheduling
{
    public class CreateAppointmentSlotResult : OperationResult
    {
        public string Id { get; }

        public CreateAppointmentSlotResult(string slotId) : base(null)
        {
            Id = slotId;
        }

        public static CreateAppointmentSlotResult Ok(string slotId)
        {
            return new CreateAppointmentSlotResult(slotId);
        }
    }
}
