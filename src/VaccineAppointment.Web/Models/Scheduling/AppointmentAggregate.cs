using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VaccineAppointment.Web.Models.Scheduling
{
    public class AppointmentAggregate
    {
        public AppointmentAggregate(LocalDateTime from, Period duration, int countOfSlot)
        {
            Slot = new AppointmentSlot()
            {
                Id = Guid.NewGuid().ToString(),
                From = from,
                Duration = duration,
                CountOfSlot = countOfSlot,
            };
        }

        public AppointmentAggregate(AppointmentSlot slot, IEnumerable<Appointment> appointments)
        {
            Slot = slot;
            Appointments.AddRange(appointments);
        }

        public string Id => Slot.Id;
        public LocalDateTime From => Slot.From;
        public Period Duration => Slot.Duration;
        public LocalDateTime To => Slot.To;
        public int CountOfSlot => Slot.CountOfSlot;

        public bool CanCreateAppointment => CountOfSlot > Appointments.Count;
        public bool CanDelete => !Appointments.Any();
        public bool CanUpdate => !Appointments.Any();

        public AppointmentSlot Slot { get; }
        public List<Appointment> Appointments { get; } = new List<Appointment>();

        public Appointment? FindAppointmentById(string id)
        {
            return Appointments.FirstOrDefault(a => a.Id == id);
        }

        public bool IsOverlap(LocalDateTime from, Period duration)
        {
            if (To <= from)
            {
                return false;
            }
            var to = from + duration;
            if (to <= From)
            {
                return false;
            }
            return true;
        }

        public void EditSlot(LocalDateTime from, Period duration, int countOfSlot)
        {
            Slot.From = from;
            Slot.Duration = duration;
            Slot.CountOfSlot = countOfSlot;
        }

        public string AddAppointment(string name, string email, string sex, int age)
        {
            var appointmentId = Guid.NewGuid().ToString();
            Appointments.Add(new Appointment()
            {
                Id = appointmentId,
                AppointmentSlotId = Id,
                From = From,
                Duration = Duration,
                Name = name,
                Email = email,
                Sex = sex,
                Age = age,
            });
            return appointmentId;
        }
    }
}
