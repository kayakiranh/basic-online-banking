using Auth.Application.DataTransferObjects;
using Auth.Application.Interfaces;
using Auth.Application.Models;
using Auth.Core.Domain;
using Helper.Infrastructure;
using Logging.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Auth.Infrastructure.Services
{
    [Serializable]
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IAccountOpenSender _accountOpenSender;
        private readonly IConfiguration _configuration;
        private readonly ILoggingService _loggingService;
        private readonly string _queueName;

        public AuthService(ApplicationDbContext context, ITokenService tokenService, IAccountOpenSender accountOpenSender, IConfiguration configuration, ILoggingService loggingService)
        {
            _context = context;
            _tokenService = tokenService;
            _accountOpenSender = accountOpenSender;
            _configuration = configuration;
            _loggingService = loggingService;
            _queueName = _configuration.GetSection("RabbitMq:QueueName").Value;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
        {
            if (!StringHelper.Validator(request.IdentityNumber, 11, 11) || !StringHelper.Validator(request.Password, 1, 30))
            {
                _loggingService.Information("Giriş alanlarını kontrol ediniz", request);
                return LoginResponseDto.ValidateError("Giriş alanlarını kontrol ediniz");
            }

            request.Password = PasswordHelper.Encrypt(request.Password);

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.IdentityNumber == request.IdentityNumber && u.Password == request.Password, cancellationToken);
            if (user == null)
            {
                _loggingService.Information("Kullanıcı bilgileriniz eşleşmiyor", request);
                return LoginResponseDto.ValidateError("Kullanıcı bilgileriniz eşleşmiyor");
            }

            TokenResponse tokenResponse = await _tokenService.GenerateToken(new TokenRequest { IdentityNumber = request.IdentityNumber });

            user.Token = tokenResponse.Token;
            user.RefreshToken = tokenResponse.RefreshToken;
            _context.Users.Update(user);
            int dbResult = await _context.SaveChangesAsync(cancellationToken);
            if (dbResult < 1)
            {
                _loggingService.Warning("İşlem yapılırken bir hata meydana geldi", request);
                return LoginResponseDto.ValidateError("İşlem yapılırken bir hata meydana geldi");
            }

            return new LoginResponseDto
            {
                AccessTokenExpireDate = tokenResponse.TokenExpireDate,
                Result = true,
                Message = string.Empty,
                AccessToken = tokenResponse.Token,
                RefreshToken = tokenResponse.RefreshToken
            };
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
        {
            if (
                !StringHelper.Validator(request.FirstName, 1, 30) ||
                !StringHelper.Validator(request.LastName, 1, 30) ||
                !StringHelper.Validator(request.IdentityNumber, 11, 11) ||
                !StringHelper.Validator(request.Password, 1, 30)
                )
            {
                _loggingService.Information("Giriş alanlarını kontrol ediniz", request);
                return RegisterResponseDto.ValidateError("Giriş alanlarını kontrol ediniz");
            }

            bool checkUserByIdentity = await _context.Users.AnyAsync(u => u.IdentityNumber == request.IdentityNumber);
            if (checkUserByIdentity)
            {
                _loggingService.Information("Daha önce kayıt oldunuz", request);
                return RegisterResponseDto.ValidateError("Daha önce kayıt oldunuz");
            }

            User user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Created = DateTime.Now,
                IdentityNumber = request.IdentityNumber,
                Password = PasswordHelper.Encrypt(request.Password),
                RefreshToken = string.Empty,
                Token = string.Empty
            };

            await _context.Users.AddAsync(user, cancellationToken);
            int dbResult = await _context.SaveChangesAsync(cancellationToken);

            if (dbResult < 1)
            {
                _loggingService.Warning("İşlem yapılırken bir hata meydana geldi", request);
                return RegisterResponseDto.ValidateError("İşlem yapılırken bir hata meydana geldi");
            }

            _accountOpenSender.OpenAccount(request);
            return new RegisterResponseDto
            {
                Message = string.Empty,
                Result = true
            };
        }
    }
}