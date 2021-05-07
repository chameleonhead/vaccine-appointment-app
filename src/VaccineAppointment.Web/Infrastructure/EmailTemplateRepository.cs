using System.Threading.Tasks;
using VaccineAppointment.Web.Models.Mailing;

namespace VaccineAppointment.Web.Infrastructure
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        public Task<EmailTemplate> FindByNameAsync(string templateName)
        {
            return Task.FromResult(new EmailTemplate()
            {
                FromTemplate = "chamelion@hotmail.co.jp",
                SubjectTemplate = "【ワクチン予約Webサイト】ご予約ありがとうございます。",
                BodyTemplate = "{{Name}}様\r\n\r\n当システムをご利用いただき、誠にありがとうございます。\r\n予約を以下の通り承りました。\r\n\r\n予約ID: {{AppointmentId}}\r\n予約日: {{Date}}\r\nお時間: {{FromTime}} - {{ToTime}}\r\n\r\n当日は所定の時間までにお越しください。\r\n\r\n本メールには返信してもお返事が出来ませんのでご了承願います。\r\n\r\n----------------------------------\r\nワクチン予約Webサイト\r\n",
            });
        }
    }
}
