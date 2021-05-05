using Microsoft.EntityFrameworkCore;
using System;
using VaccineAppointment.Web.Infrastructure;

namespace VaccineAppointment.Web.Tests
{
    static class Utils
    {
        public static VaccineAppointmentContext CreateInMemoryContext()
        {
            return new VaccineAppointmentContext(new DbContextOptionsBuilder<VaccineAppointmentContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }
    }
}
