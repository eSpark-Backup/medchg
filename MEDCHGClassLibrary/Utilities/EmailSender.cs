using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Models;

namespace MEDCHGClassLibrary.Utilities
{
    public class EmailSender
    {

        public static bool Send(EmailSend emailSend)

        {

            try
            {

                var fromAddress = new MailAddress(emailSend.SenderAddress, emailSend.SenderName);
                var toAddress = new MailAddress(emailSend.ReceiverAddress, emailSend.ReceiverName);
                string fromPassword = emailSend.SenderPassword;
                string subject = emailSend.Subject;
                string body = emailSend.Body;//"\"Hi \" + \"Suresh\" + \"!\\n\" + \"Welcome to C# Corner!\" + \"Please <a href='\" + \" http://windowsapptutorials.wordpress.com/ \" + \"'>click</a> here to activate your account. \\nThanks!\";";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    //Host = "smtp.live.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    //IsBodyHtml = true,
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                    return true;
                }

            }
            catch (Exception ex)
            {

                ex.Message.ToString();
                return false;
            }

        }


    }
}
