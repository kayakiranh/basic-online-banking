using Account.Application.DataTransferObjects;
using Account.Application.Interfaces;
using Moq;
using Xunit;

namespace Account.Test
{
    [Serializable]
    public class WithdrawTest
    {
        private readonly CancellationToken _cancellationToken = new CancellationToken();
        private readonly Mock<IAccountService> _accountService;

        public WithdrawTest()
        {
            _accountService = new Mock<IAccountService>();
        }

        [Fact]
        public async void Success()
        {
            AccountWithdrawRequestDto accountWithdrawRequestDto = new AccountWithdrawRequestDto
            {
                AccountNumber = "00000000000000000000000000000",
                Amount = 100,
                From = "11111111111111111111111111111",
            };
            AccountWithdrawResponseDto accountWithdrawResponseDto = await _accountService.Object.WithdrawAsync(accountWithdrawRequestDto, _cancellationToken);
            Assert.NotEqual(accountWithdrawResponseDto.AccountNumber, string.Empty);
        }

        [Fact]
        public async void NotEnoughBalance()
        {
            AccountWithdrawRequestDto accountWithdrawRequestDto = new AccountWithdrawRequestDto
            {
                AccountNumber = "00000000000000000000000000000",
                Amount = 9999999,
                From = "11111111111111111111111111111",
            };
            AccountWithdrawResponseDto accountWithdrawResponseDto = await _accountService.Object.WithdrawAsync(accountWithdrawRequestDto, _cancellationToken);
            Assert.NotEqual(accountWithdrawResponseDto.AccountNumber, string.Empty);
        }

        [Fact]
        public async void NotFound()
        {
            AccountWithdrawRequestDto accountWithdrawRequestDto = new AccountWithdrawRequestDto
            {
                AccountNumber = "00000000000000000000000000000",
                Amount = 100,
                From = "11111111111111111111111111110",
            };
            AccountWithdrawResponseDto accountWithdrawResponseDto = await _accountService.Object.WithdrawAsync(accountWithdrawRequestDto, _cancellationToken);
            Assert.Equal(accountWithdrawResponseDto.AccountNumber, string.Empty);
        }

        [Fact]
        public async void Validation()
        {
            AccountWithdrawRequestDto accountWithdrawRequestDto = new AccountWithdrawRequestDto
            {
                AccountNumber = "000",
                Amount = -1,
                From = "111",
            };
            AccountWithdrawResponseDto accountWithdrawResponseDto = await _accountService.Object.WithdrawAsync(accountWithdrawRequestDto, _cancellationToken);
            Assert.Equal(accountWithdrawResponseDto.AccountNumber, string.Empty);
        }
    }
}