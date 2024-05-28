using Auth.Application.Interfaces;
using Auth.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<TokenResponse> GenerateToken(TokenRequest request)
        {
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]));

            var dateTimeNow = DateTime.Now;

            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                claims: new List<Claim> {
                    new Claim("IdentityNumber", request.IdentityNumber)
                },
                notBefore: DateTime.Now,
                expires: dateTimeNow.Add(TimeSpan.FromMinutes(Convert.ToInt32(_configuration["Jwt:ExpireDuration"]))),
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );

            JwtSecurityToken refreshJwt = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                claims: new List<Claim> {
                                new Claim("IdentityNumber", request.IdentityNumber)
                },
                notBefore: DateTime.Now,
                expires: dateTimeNow.Add(TimeSpan.FromMinutes(Convert.ToInt32(_configuration["Jwt:RefreshExpireDuration"]))),
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );

            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                TokenExpireDate = DateTime.Now.AddSeconds(Convert.ToInt16(30)),
                RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshJwt),
            };
        }
    }
}