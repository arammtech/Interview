using Microsoft.AspNetCore.Identity;
using System.Text;
using Interview.Domain.Identity;
using Interview.Service.Interfaces;

namespace Interview.Service.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string DecodeShortenToken(string shortToken)
        {
            string paddedToken = shortToken.Replace('-', '+').Replace('_', '/');

            while (paddedToken.Length % 4 != 0)
                paddedToken += "="; // Restore padding

            var tokenBytes = Convert.FromBase64String(paddedToken);
            return Encoding.UTF8.GetString(tokenBytes);
        }

        public async Task<string> GenerateToken(ApplicationUser user)
        {
            if (user == null) return null;
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public string ShortenToken(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(tokenBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }
    }
}
