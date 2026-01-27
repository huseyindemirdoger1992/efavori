using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using System.Linq; // Dosya isimlerini birleştirmek için
using data; // EmailHistory sınıfının bulunduğu namespace

namespace api
{
    public class EmailSender
    {
        private const string SmtpServer = "srvm16.trwww.com";
        private const int SmtpPort = 587;
        private const string EmailAddress = "security@efavori.com";
        private const string EmailPassword = "SXAk4obokyOePCJ48VsR";

        private readonly TakeLogs _logger;
        private readonly _ApplicationConnectionDb _context;

        public EmailSender(TakeLogs logger, _ApplicationConnectionDb context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task SendEmailAsync(string recipient, string subject, string body, List<Attachment>? attachments = null)
        {
            // Kayıt için kullanılacak ek dosya isimleri
            string attachmentNames = attachments != null
                ? string.Join(", ", attachments.Select(a => a.Name))
                : string.Empty;

            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(EmailAddress, "Efavori Security");
                message.To.Add(recipient);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var attachment in attachments)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                using var smtp = new SmtpClient(SmtpServer, SmtpPort)
                {
                    Credentials = new NetworkCredential(EmailAddress, EmailPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                await smtp.SendMailAsync(message);

                // --- KAYIT İŞLEMİ BAŞLIYOR ---
                var history = new EmailHistory
                {
                    FromWhom = EmailAddress,
                    ToWho = recipient,
                    Subject = subject,
                    Body = body,
                    Attachments = attachmentNames,
                    SentDate = DateTime.UtcNow
                };

                _context.EmailHistory.Add(history);
                await _context.SaveChangesAsync();
                // --- KAYIT İŞLEMİ BİTTİ ---
            }
            catch (Exception ex)
            {
                try
                {
                    await _logger.TakeIt(
                        userId: null,
                        PageNameSpaceTitle: "namespace api",
                        action: "SendEmailAsync",
                        exception: ex.Message,
                        stackTrace: ex.StackTrace
                    );
                }
                catch { }
            }
        }
    }
}