using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Threading.Tasks;
using VaccineAppointment.Web.Infrastructure;
using VaccineAppointment.Web.Models.Mailing;
using VaccineAppointment.Web.Services.Mailing;

namespace VaccineAppointment.Web.Tests
{
    static class Utils
    {
        public static VaccineAppointmentContext CreateInMemoryContext()
        {
            return new VaccineAppointmentContext(new DbContextOptionsBuilder<VaccineAppointmentContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(options =>
                {
                    options.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                })
                .Options);
        }

        public static EmailService CreateMockEmailService()
        {
            return new EmailService(new MockEmailSender(), new MockEmailTemplateRepository());
        }

        private class MockEmailSender : IEmailSender
        {
            public Task SendMailAsync(EmailMessage param)
            {
                return Task.CompletedTask;
            }
        }

        private class MockEmailTemplateRepository : IEmailTemplateRepository
        {
            public Task<EmailTemplate> FindByNameAsync(string templateName)
            {
                return Task.FromResult(EmailTemplate.Default);
            }

            public Task SaveAsync(EmailTemplate template)
            {
                return Task.CompletedTask;
            }
        }
    }
}
