using NodaTime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public interface IAppointmentAggregateRepository
    {
        Task<Appointment> FindByIdAsync(string id);
        Task<List<Appointment>> SearchAppointmentsFor(LocalDateTime from, LocalDateTime to);
        Task AddAsync(Appointment user);
        Task UpdateAsync(Appointment user);
    }
}
