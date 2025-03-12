using Muqabalati.Domain.Identity;

namespace Muqabalati.Service.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        string ShortenToken(string token);
        string DecodeShortenToken(string shortToken);
    }
}
