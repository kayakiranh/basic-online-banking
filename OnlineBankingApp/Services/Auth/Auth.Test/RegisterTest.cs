using Auth.Application.DataTransferObjects;
using Auth.Application.Interfaces;
using Moq;
using Xunit;

namespace Auth.Test
{
    public class RegisterTest
    {
        private readonly CancellationToken _cancellationToken = new CancellationToken();
        private readonly Mock<IAuthService> _authService;

        public RegisterTest()
        {
            _authService = new Mock<IAuthService>();
        }

        [Fact]
        public async void Success()
        {
            RegisterRequestDto registerRequestDto = new RegisterRequestDto
            {
                IdentityNumber = "00000000000",
                Password = "123456",
                FirstName = "Hüseyin",
                LastName = "Kayakıran"
            };
            RegisterResponseDto registerResultDto = await _authService.Object.RegisterAsync(registerRequestDto, _cancellationToken);
            Assert.True(registerResultDto.Result);
        }

        [Fact]
        public async void NotFound()
        {
            RegisterRequestDto registerRequestDto = new RegisterRequestDto
            {
                IdentityNumber = "11111111111",
                Password = "123456",
                FirstName = "Hüseyin",
                LastName = "Kayakıran"
            };
            RegisterResponseDto registerResultDto = await _authService.Object.RegisterAsync(registerRequestDto, _cancellationToken);
            Assert.True(registerResultDto.Result);
        }

        [Fact]
        public async void Validation()
        {
            RegisterRequestDto registerRequestDto = new RegisterRequestDto
            {
                IdentityNumber = "000",
                Password = "",
                FirstName = "",
                LastName = ""
            };
            RegisterResponseDto registerResultDto = await _authService.Object.RegisterAsync(registerRequestDto, _cancellationToken);
            Assert.True(registerResultDto.Result);
        }
    }
}