
using Muqabalati.Domain.Global;

namespace Muqabalati.Service.EmailService
{
    public interface IEmailSenderStrategy
    {
        Task<Result> SendEmailAsync(string toMail,string toName, string subject, string body);
    }
}
