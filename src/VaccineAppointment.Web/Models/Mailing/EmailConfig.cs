using System;

namespace VaccineAppointment.Web.Models.Mailing
{
    public class EmailConfig
    {
        public string? Host { get; set; }
        public int? Port { get; set; }
        public bool UseSsl { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
