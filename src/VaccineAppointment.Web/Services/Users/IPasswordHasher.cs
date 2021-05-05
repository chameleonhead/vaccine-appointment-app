namespace VaccineAppointment.Web.Services.Users
{
    public interface IPasswordHasher
    {
        string Hash(string password);
    }
}
