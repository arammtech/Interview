using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Interview.Domain.Entities;
using Interview.Domain.Global;

namespace Interview.Service.EmailService
{
    public class SmtpEmailSender : IEmailSenderStrategy
    {

        private readonly EmailSettings _emailSettings;

        public SmtpEmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<Result> SendEmailAsync(string toMail, string toName, string subject, string body)
        {
            try
            {
                using (var _mailMessage = new MailMessage())
                {
                    _mailMessage.From = new MailAddress(_emailSettings.SenderEmail,_emailSettings.SenderName);
                    _mailMessage.To.Add(new MailAddress(toMail,toName));
                    _mailMessage.Subject = subject;
                    _mailMessage.Body = body;
                    _mailMessage.IsBodyHtml = true;

                    using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                    {
                        smtpClient.EnableSsl = true;
                        smtpClient.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.AppPassword);
                        await smtpClient.SendMailAsync(_mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
            return Result.Success();
        }
    }
}
