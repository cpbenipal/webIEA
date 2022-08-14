using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace webIEA.Repositories
{
    public class Email
    {
       
        public static int SendEmail(string receiver, string subject, string message)
        {
            try
            {
                var client = new SmtpClient("smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential("774b0db0edf0ce", "089911836de727"),
                    EnableSsl = true
                };
                client.Send("from@example.com", receiver,subject, message);
                //var senderEmail = new MailAddress("774b0db0edf0ce", "089911836de727");
                //    var receiverEmail = new MailAddress(receiver, "Receiver");
                //    var password = "Your Email Password here";
                //    var sub = subject;
                //    var body = message;
                //    var smtp = new SmtpClient
                //    {
                //        Host = "smtp.mailtrap.io",
                //        Port = 2525,
                //        EnableSsl = true,
                //        Credentials = new NetworkCredential(senderEmail.Address, password)
                //    };
                //    using (var mess = new MailMessage(senderEmail, receiverEmail)
                //    {
                //        Subject = subject,
                //        Body = body
                //    })
                //    {
                //        smtp.Send(mess);
                //    }
                    return 1;
                
            }
            catch (Exception)
            {
                return 0;
            }
           
        }
    }
}
