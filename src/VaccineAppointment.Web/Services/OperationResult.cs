namespace VaccineAppointment.Web.Services
{
    public class OperationResult
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }

        public OperationResult(string? errorMessage = null)
        {
            Succeeded = string.IsNullOrEmpty(errorMessage);
            ErrorMessage = errorMessage;
        }

        public static OperationResult Ok()
        {
            return new OperationResult();
        }

        public static OperationResult Fail(string errorMessage)
        {
            return new OperationResult(errorMessage);
        }
    }
}
