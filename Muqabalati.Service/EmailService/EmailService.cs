using Muqabalati.Domain.Global;

namespace Muqabalati.Service.EmailService
{
    public class EmailService
    {
        private IEmailSenderStrategy _emailSenderStrategy;

        public EmailService(IEmailSenderStrategy emailSenderStrategy)
        {
            _emailSenderStrategy = emailSenderStrategy;
        }

        public async Task<Result> SendEmailAsync(string toMail, string toName, string subject, string body)
        {
            return await _emailSenderStrategy.SendEmailAsync(toMail,toName, subject, body);
        }

        public void SwitchEmailSenderStrategy(IEmailSenderStrategy emailSenderStrategy)
        {
            _emailSenderStrategy = emailSenderStrategy;
        }
    }
}



