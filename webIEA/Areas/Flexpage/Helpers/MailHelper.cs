using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Flexpage.Abstract;

namespace FlexPage.Helpers
{
    public class MailHelper : IMailSender
    {
        public async Task<bool> SendMail(string fromAddress, string toAddress, string subject, string body)
        {
            try
            {

                //using (SmtpClient client = new SmtpClient())
                //{
                //    var mail = new MimeMessage();
                //    mail.From.Add(new MailboxAddress("finnwedenn@gmail.com"));
                //    mail.To.Add(new MailboxAddress("ifetisov@nic-starc.ru"));
                //    mail.Subject = subject;
                //    var bb = new BodyBuilder();
                //    bb.HtmlBody = body;
                //    mail.Body = bb.ToMessageBody();

                //    client.Connect("smtp.gmail.com", 587);
                //    client.AuthenticationMechanisms.Remove("XOAUTH2");

                //    // Note: only needed if the SMTP server requires authentication
                //    client.Authenticate("finnwedenn@gmail.com", "jermaa1979");

                //    client.Send(mail);
                //    client.Disconnect(true);
                //}

                // using (var client = new SmtpClient("smtp.yandex.ru", 587))
                using (var client = new SmtpClient())
                {
                    var mail = new MailMessage(fromAddress, toAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    client.Send(mail);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}