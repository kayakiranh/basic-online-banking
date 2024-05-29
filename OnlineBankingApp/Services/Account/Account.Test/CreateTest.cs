using Account.Application.DataTransferObjects;
using Account.Application.Interfaces;
using Moq;
using Xunit;

namespace Account.Test
{
    [Serializable]
    public class CreateTest
    {
        private readonly CancellationToken _cancellationToken = new CancellationToken();
        private readonly Mock<IAccountService> _accountService;

        public CreateTest()
        {
            _accountService = new Mock<IAccountService>();
        }

        [Fact]
        public async void Success()
        {
            AccountCreateRequestDto accountCreateRequestDto = new AccountCreateRequestDto
            {
                AccountNumber = "00000000000000000000000000000",
                AccountHolderName = "Hüseyin Kayakıran",
                Balance = 0
            };
            AccountCreateResponseDto accountCreateResponseDto = await _accountService.Object.CreateAsync(accountCreateRequestDto, _cancellationToken);
            Assert.NotEqual(accountCreateResponseDto.AccountNumber, string.Empty);
        }

        [Fact]
        public async void NotFound()
        {
            AccountCreateRequestDto accountCreateRequestDto = new AccountCreateRequestDto
            {
                AccountNumber = "1111111111111111111111111111111",
                AccountHolderName = "Hüseyin Kayakıran",
                Balance = 0
            };
            AccountCreateResponseDto accountCreateResponseDto = await _accountService.Object.CreateAsync(accountCreateRequestDto, _cancellationToken);
            Assert.Equal(accountCreateResponseDto.AccountNumber, string.Empty);
        }

        [Fact]
        public async void Validation()
        {
            AccountCreateRequestDto accountCreateRequestDto = new AccountCreateRequestDto
            {
                AccountNumber = "000",
                AccountHolderName = "Hüseyin Kayakıran",
                Balance = 0
            };
            AccountCreateResponseDto accountCreateResponseDto = await _accountService.Object.CreateAsync(accountCreateRequestDto, _cancellationToken);
            Assert.Equal(accountCreateResponseDto.AccountNumber, string.Empty);
        }
    }
}