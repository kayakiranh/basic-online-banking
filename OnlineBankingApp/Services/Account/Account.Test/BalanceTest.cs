using Account.Application.DataTransferObjects;
using Account.Application.Interfaces;
using Moq;
using Xunit;

namespace Account.Test
{
    [Serializable]
    public class BalanceTest
    {
        private readonly CancellationToken _cancellationToken = new CancellationToken();
        private readonly Mock<IAccountService> _accountService;

        public BalanceTest()
        {
            _accountService = new Mock<IAccountService>();
        }

        [Fact]
        public async void Success()
        {
            AccountBalanceRequestDto accountBalanceRequestDto = new AccountBalanceRequestDto
            {
                AccountNumber = "00000000000000000000000000000"
            };
            AccountBalanceResponseDto accountBalanceResponseDto = await _accountService.Object.BalanceAsync(accountBalanceRequestDto, _cancellationToken);
            Assert.NotEmpty(accountBalanceResponseDto.Items);
        }

        [Fact]
        public async void NotFound()
        {
            AccountBalanceRequestDto accountBalanceRequestDto = new AccountBalanceRequestDto
            {
                AccountNumber = "1111111111111111111111111111111"
            };
            AccountBalanceResponseDto accountBalanceResponseDto = await _accountService.Object.BalanceAsync(accountBalanceRequestDto, _cancellationToken);
            Assert.Empty(accountBalanceResponseDto.Items);
        }

        [Fact]
        public async void Validation()
        {
            AccountBalanceRequestDto accountBalanceRequestDto = new AccountBalanceRequestDto
            {
                AccountNumber = "000"
            };
            AccountBalanceResponseDto accountBalanceResponseDto = await _accountService.Object.BalanceAsync(accountBalanceRequestDto, _cancellationToken);
            Assert.Empty(accountBalanceResponseDto.Items);
        }
    }
}