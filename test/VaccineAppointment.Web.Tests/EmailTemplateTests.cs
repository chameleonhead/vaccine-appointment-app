using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using VaccineAppointment.Web.Models.Mailing;
using VaccineAppointment.Web.Models.Mailing.MessageParams;

namespace VaccineAppointment.Web.Tests
{
    [TestClass]
    public class EmailTemplateTests
    {
        [TestMethod]
        public void CreateMessage_should_return_message()
        {
            var template = new EmailTemplate()
            {
                FromTemplate = "chamelion@hotmail.co.jp",
                SubjectTemplate = "",
                BodyTemplate = "{{Name}}様\r\n\r\n当システムをご利用いただき、誠にありがとうございます。\r\n予約を以下の通り承りました。\r\n\r\n予約ID: {{AppointmentId}}\r\n予約日: {{Date}}\r\nお時間: {{FromTime}} - {{ToTime}}\r\n\r\n当日は所定の時間までにお越しください。\r\n\r\n本メールには返信してもお返事が出来ませんのでご了承願います。\r\n\r\n----------------------------------\r\nワクチン予約Webサイト\r\n",
            };
            var message = template.CreateMessage(new EmailMessageParams()
            {
                To = "user@example.com",
                Params = new AppointmentAcceptedMessageParams("id", new LocalDate(2021, 5, 5), new LocalTime(10, 0), new LocalTime(11, 0), "User"),
            });
            Assert.AreEqual(template.FromTemplate, message.From);
            Assert.AreEqual("user@example.com", message.To);
            Assert.AreEqual(@"User様

当システムをご利用いただき、誠にありがとうございます。
予約を以下の通り承りました。

予約ID: id
予約日: 2021年5月5日
お時間: 10:00:00 - 11:00:00

当日は所定の時間までにお越しください。

本メールには返信してもお返事が出来ませんのでご了承願います。

----------------------------------
ワクチン予約Webサイト
", message.Body);
        }
    }
}
