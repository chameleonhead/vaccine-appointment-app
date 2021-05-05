using NodaTime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaccineAppointment.Web.Models.Scheduling
{
    interface IAppointmentRepository
    {
        Task<Appointment> FindByIdAsync(string id);
        Task<List<Appointment>> SearchAppointmentsFor(LocalDateTime from, LocalDateTime to);
        Task SaveAsync(Appointment user);
    }
}
