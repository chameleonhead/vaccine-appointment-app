using System;

namespace VaccineAppointment.Web.Models.Users
{
    public class User
    {
        public User()
        {
            Id = Guid.Empty.ToString();
            Username = "Unknown";
            Role = "User";
        }

        public User(string username, string password, string role)
        {
            Id = Guid.NewGuid().ToString();
            Username = username;
            Password = password;
            Role = role;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; }

        public void ChangePassword(string password)
        {
            Password = password;
        }

        public void ChangeRole(string role)
        {
            Role = role;
        }
    }
}
