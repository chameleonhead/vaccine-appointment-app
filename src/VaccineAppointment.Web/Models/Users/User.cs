﻿using System;

namespace VaccineAppointment.Web.Models.Users
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
            Username = "Unknown";
            Role = "User";
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; }
    }
}