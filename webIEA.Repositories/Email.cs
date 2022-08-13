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
                    var senderEmail = new MailAddress("jamilmoughal786@gmail.com", "Jamil");
                    var receiverEmail = new MailAddress(receiver, "Receiver");
                    var password = "Your Email Password here";
                    var sub = subject;
                    var body = message;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(mess);
                    }
                    return 1;
                
            }
            catch (Exception)
            {
                return 0;
            }
           
        }
    }
}
