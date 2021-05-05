using VaccineAppointment.Web.Models.Users;

namespace VaccineAppointment.Web.Services.Users
{
    public class ValidateUsernameAndPasswordResult : OperationResult
    {
        public User User { get; }

        public ValidateUsernameAndPasswordResult(User user) : base(null)
        {
            User = user;
        }

        public static ValidateUsernameAndPasswordResult Ok(User user)
        {
            return new ValidateUsernameAndPasswordResult(user);
        }
    }
}