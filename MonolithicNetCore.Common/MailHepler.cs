using System.Net;
using System.Net.Mail;

namespace MonolithicNetCore.Common
{
    public class MailHepler
    {
        public static void SendMail(string toMail, string subject, string body)
        {
            using (var message = new MailMessage())
            {
                message.To.Add(new MailAddress(toMail));
                message.From = new MailAddress(ConfigAppSetting.FromEmail);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential(ConfigAppSetting.FromEmail, ConfigAppSetting.EmailPass);
                    client.EnableSsl = true;
                    client.Send(message);
                }
            }
        }
    }
}
