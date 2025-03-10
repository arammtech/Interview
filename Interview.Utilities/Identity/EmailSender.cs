using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Interview.Domain.Entities;
using Interview.Domain.Identity;

namespace Interview.Utilities.Identity
{
    public class EmailSender : IEmailSender
    {
        // Should be encrypted Later
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "teamaramm@gmail.com";
        private readonly string _smtpPass = "iweq ccca vufw gbnb";
        private readonly string _fromEmail = "teamaramm@gmail.com";

        //private readonly EmailSettings _emailSettings;
        //public EmailSender(IOptions<EmailSettings> emailSettings, UserManager<ApplicationUser> userManger)
        //{
        //    _emailSettings = emailSettings.Value;
        //}

        //public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    using (var mailMessage = new MailMessage())
        //    {
        //        mailMessage.From = new MailAddress(_emailSettings.SenderEmail);
        //        mailMessage.To.Add(email);
        //        mailMessage.Subject = subject;
        //        mailMessage.Body = htmlMessage;
        //        mailMessage.IsBodyHtml = true;

        //        using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
        //        {
        //            smtpClient.EnableSsl = true;
        //            smtpClient.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.AppPassword);
        //            await smtpClient.SendMailAsync(mailMessage);
        //        }
        //    }
        //}
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_fromEmail);
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = htmlMessage;
                mailMessage.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }
    }
}
