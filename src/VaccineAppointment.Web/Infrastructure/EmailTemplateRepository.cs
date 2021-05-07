using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Mailing;

namespace VaccineAppointment.Web.Infrastructure
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly VaccineAppointmentContext db;

        public EmailTemplateRepository(VaccineAppointmentContext db)
        {
            this.db = db;
        }

        public Task<EmailTemplate> FindByNameAsync(string templateName)
        {
            return db.EmailTemplates.FirstOrDefaultAsync(t => t.TemplateName == templateName);
        }

        public async Task SaveAsync(EmailTemplate template)
        {
            var dbTemplate = await FindByNameAsync(template.TemplateName);
            using var trans = await db.Database.BeginTransactionAsync();
            if (dbTemplate != null)
            {
                template.Id = dbTemplate.Id;
                db.EmailTemplates.Remove(dbTemplate);
                await db.SaveChangesAsync();
            }
            else
            {
                template.Id = Guid.NewGuid().ToString();
            }
            await db.EmailTemplates.AddAsync(template);
            await db.SaveChangesAsync();
            await trans.CommitAsync();
        }
    }
}
