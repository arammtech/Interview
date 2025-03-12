using Muqabalati.Domain.Global;

namespace Muqabalati.Service.Interfaces
{
    public interface IEmailVerificationService
    {
        Task<Result> VerifyEmailAsync(int userId, string token);
        string GenerateLinkToVerifyTokenAsync(string token, int userId);
    }
}
