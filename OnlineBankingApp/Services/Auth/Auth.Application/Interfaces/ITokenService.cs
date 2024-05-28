using Auth.Application.Models;

namespace Auth.Application.Interfaces
{
    public interface ITokenService
    {
        public Task<TokenResponse> GenerateToken(TokenRequest request);
    }
}