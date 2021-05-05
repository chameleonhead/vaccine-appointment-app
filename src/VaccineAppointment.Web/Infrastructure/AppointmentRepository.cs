using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Scheduling;

namespace VaccineAppointment.Web.Infrastructure
{
    public class AppointmentRepository : IAppointmentAggregateRepository
    {
        public Task<List<Appointment>> SearchAppointmentsFor(LocalDateTime from, LocalDateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<Appointment> FindByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Appointment user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Appointment user)
        {
            throw new NotImplementedException();
        }
    }
}
