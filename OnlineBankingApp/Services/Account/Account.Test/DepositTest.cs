using Account.Application.DataTransferObjects;
using Account.Application.Interfaces;
using Moq;
using Xunit;

namespace Account.Test
{
    [Serializable]
    public class DepositTest
    {
        private readonly CancellationToken _cancellationToken = new CancellationToken();
        private readonly Mock<IAccountService> _accountService;

        public DepositTest()
        {
            _accountService = new Mock<IAccountService>();
        }

        [Fact]
        public async void Success()
        {
            AccountDepositRequestDto accountDepositRequestDto = new AccountDepositRequestDto
            {
                AccountNumber = "00000000000000000000000000000",
                Amount = 100,
                From = "11111111111111111111111111111",
            };
            AccountDepositResponseDto accountDepositResponseDto = await _accountService.Object.DepositAsync(accountDepositRequestDto, _cancellationToken);
            Assert.NotEqual(accountDepositResponseDto.AccountNumber, string.Empty);
        }

        [Fact]
        public async void NotEnoughBalance()
        {
            AccountDepositRequestDto accountDepositRequestDto = new AccountDepositRequestDto
            {
                AccountNumber = "00000000000000000000000000000",
                Amount = 9999999,
                From = "11111111111111111111111111111",
            };
            AccountDepositResponseDto accountDepositResponseDto = await _accountService.Object.DepositAsync(accountDepositRequestDto, _cancellationToken);
            Assert.NotEqual(accountDepositResponseDto.AccountNumber, string.Empty);
        }

        [Fact]
        public async void NotFound()
        {
            AccountDepositRequestDto accountDepositRequestDto = new AccountDepositRequestDto
            {
                AccountNumber = "00000000000000000000000000000",
                Amount = 100,
                From = "11111111111111111111111111110",
            };
            AccountDepositResponseDto accountDepositResponseDto = await _accountService.Object.DepositAsync(accountDepositRequestDto, _cancellationToken);
            Assert.Equal(accountDepositResponseDto.AccountNumber, string.Empty);
        }

        [Fact]
        public async void Validation()
        {
            AccountDepositRequestDto accountDepositRequestDto = new AccountDepositRequestDto
            {
                AccountNumber = "000",
                Amount = -1,
                From = "111",
            };
            AccountDepositResponseDto accountDepositResponseDto = await _accountService.Object.DepositAsync(accountDepositRequestDto, _cancellationToken);
            Assert.Equal(accountDepositRequestDto.AccountNumber, string.Empty);
        }
    }
}