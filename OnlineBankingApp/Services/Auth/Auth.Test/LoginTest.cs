using Auth.Application.DataTransferObjects;
using Auth.Application.Interfaces;
using Moq;
using Xunit;

namespace Auth.Test
{
    public class LoginTest
    {
        private readonly CancellationToken _cancellationToken = new CancellationToken();
        private readonly Mock<IAuthService> _authService;

        public LoginTest()
        {
            _authService = new Mock<IAuthService>();
        }

        [Fact]
        public async void Success()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto
            {
                IdentityNumber = "00000000000",
                Password = "123456"
            };
            LoginResponseDto loginResponseDto = await _authService.Object.LoginAsync(loginRequestDto, _cancellationToken);
            Assert.True(loginResponseDto.Result);
        }

        [Fact]
        public async void NotFound()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto
            {
                IdentityNumber = "11111111111",
                Password = "123456"
            };
            LoginResponseDto loginResponseDto = await _authService.Object.LoginAsync(loginRequestDto, _cancellationToken);
            Assert.True(loginResponseDto.Result);
        }

        [Fact]
        public async void Validation()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto
            {
                IdentityNumber = "000",
                Password = ""
            };
            LoginResponseDto loginResponseDto = await _authService.Object.LoginAsync(loginRequestDto, _cancellationToken);
            Assert.True(loginResponseDto.Result);
        }
    }
}