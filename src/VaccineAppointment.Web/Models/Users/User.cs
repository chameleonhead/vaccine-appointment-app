using System;

namespace VaccineAppointment.Web.Models.Users
{
    public class User
    {
        public User()
        {
            Id = Guid.Empty.ToString();
            Name = "Unknown";
            Username = "unknown";
            Role = "User";
        }

        public User(string username, string password, string role, string name)
        {
            Id = Guid.NewGuid().ToString();
            Username = username;
            Password = password;
            Role = role;
            Name = name;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; }

        public void ChangeName(string name)
        {
            Name = name;
        }

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
