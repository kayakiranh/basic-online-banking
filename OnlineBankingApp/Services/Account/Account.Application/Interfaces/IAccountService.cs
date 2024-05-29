using Account.Application.DataTransferObjects;

namespace Account.Application.Interfaces
{
    public interface IAccountService
    {
        Task<AccountCreateResponseDto> CreateAsync(AccountCreateRequestDto request, CancellationToken cancellationToken);

        Task<AccountDepositResponseDto> DepositAsync(AccountDepositRequestDto request, CancellationToken cancellationToken);

        Task<AccountWithdrawResponseDto> WithdrawAsync(AccountWithdrawRequestDto request, CancellationToken cancellationToken);

        Task<AccountBalanceResponseDto> BalanceAsync(AccountBalanceRequestDto request, CancellationToken cancellationToken);
    }
}