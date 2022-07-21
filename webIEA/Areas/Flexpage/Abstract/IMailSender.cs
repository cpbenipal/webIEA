using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flexpage.Abstract
{
    interface IMailSender
    {
        Task<bool> SendMail(string fromAddress, string toAddress, string subject, string body);
    }
}
