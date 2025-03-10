using Interview.Domain.Identity;

namespace Interview.Service.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        string ShortenToken(string token);
        string DecodeShortenToken(string shortToken);
    }
}
