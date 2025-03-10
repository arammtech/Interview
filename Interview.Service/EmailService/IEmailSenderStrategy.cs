
using Interview.Domain.Global;

namespace Interview.Service.EmailService
{
    public interface IEmailSenderStrategy
    {
        Task<Result> SendEmailAsync(string toMail,string toName, string subject, string body);
    }
}
