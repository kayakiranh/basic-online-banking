using Auth.Application.DataTransferObjects;

namespace Auth.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);

        public Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
    }
}