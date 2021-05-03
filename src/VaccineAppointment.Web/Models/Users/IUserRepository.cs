﻿using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Users
{
    public interface IUserRepository
    {
        Task<User> FindByUsernameAsync(string username);
        Task SaveAsync(User user);
    }
}