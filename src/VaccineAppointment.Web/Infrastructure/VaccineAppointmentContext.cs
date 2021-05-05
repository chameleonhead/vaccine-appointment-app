using Microsoft.EntityFrameworkCore;
using VaccineAppointment.Web.Models.Scheduling;
using VaccineAppointment.Web.Models.Users;

namespace VaccineAppointment.Web.Infrastructure
{
    public class VaccineAppointmentContext : DbContext
    {
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public VaccineAppointmentContext(DbContextOptions<VaccineAppointmentContext> options) : base(options)
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<AppointmentSlot> Slots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentConfig> AppointmentConfig { get; set; }
    }
}
