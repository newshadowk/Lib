using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Base
{
    public static class Mail
    {
        public static void SendMail(string smtpServer, string password, string fromAddress, string subject,
                                    string body, params string[] toAddresses)
        {
            var client = new SmtpClient(smtpServer);
            client.Credentials = new NetworkCredential(fromAddress, password);
            client.EnableSsl = true;

            var message = new MailMessage();
            message.From = new MailAddress(fromAddress);
            foreach (string s in toAddresses)
                message.To.Add(new MailAddress(s));
            message.Subject = subject;
            message.Body = body;

            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;
            message.IsBodyHtml = false;
            message.Priority = MailPriority.Normal;

            client.Send(message);
        }
    }
}
