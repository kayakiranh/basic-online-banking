using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Helper.Infrastructure
{
    public static class PasswordHelper
    {
        public static string Encrypt(this string password)
        {
            //https://learn.microsoft.com/tr-tr/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-7.0
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}