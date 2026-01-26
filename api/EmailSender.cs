using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace api
{
    public class EmailSender
    {
        private const string SmtpServer = "srvm16.trwww.com";
        private const int SmtpPort = 465;
        private const string EmailAddress = "security@efavori.com";
        private const string EmailPassword = "SXAk4obokyOePCJ48VsR";
        private readonly TakeLogs _logger; // Logging servisi

        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(EmailAddress, "Efavori Security");
                message.To.Add(recipient);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true; // HTML desteklesin

                using var smtp = new SmtpClient(SmtpServer, SmtpPort)
                {
                    Credentials = new NetworkCredential(EmailAddress, EmailPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Loglaman gereken yer
                await _logger.TakeIt(
                    userId: null,
                    PageNameSpaceTitle: "namespace api",
                    action: $"SendEmailAsync",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
        }
    }
}
