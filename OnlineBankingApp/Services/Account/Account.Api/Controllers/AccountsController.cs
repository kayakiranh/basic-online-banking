using Account.Application.DataTransferObjects;
using Account.Application.Interfaces;
using Common.Infrastructure.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace Account.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("~/")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] AccountCreateRequestDto model, CancellationToken cancellationToken)
        {
            Task<AccountCreateResponseDto> response = Policy.HandleResult<AccountCreateResponseDto>(x => string.IsNullOrEmpty(x.AccountNumber))
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(30), (_, timeSpan, retryCount, _) => Console.WriteLine($"Retry : {retryCount}, Waiting : {timeSpan} "))
            .ExecuteAsync(async () => await _accountService.CreateAsync(model, cancellationToken));

            return ResponseDto.Success(response, string.IsNullOrEmpty(response.Result.AccountNumber), "İşlem yapılırken bir hata meydana geldi");
        }

        [HttpPost("{id}/deposit")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<ActionResult<ResponseDto>> Deposit([FromBody] AccountDepositRequestDto model, CancellationToken cancellationToken)
        {
            Task<AccountDepositResponseDto> response = Policy.HandleResult<AccountDepositResponseDto>(x => x.Amount == 0)
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(30), (_, timeSpan, retryCount, _) => Console.WriteLine($"Retry : {retryCount}, Waiting : {timeSpan} "))
            .ExecuteAsync(async () => await _accountService.DepositAsync(model, cancellationToken));

            return ResponseDto.Success(response, response.Result.Amount != 0, "İşlem yapılırken bir hata meydana geldi");
        }

        [HttpPost("{id}/withdraw")]
        [MapToApiVersion("1")]
        public async Task<ActionResult<ResponseDto>> Withdraw([FromBody] AccountWithdrawRequestDto model, CancellationToken cancellationToken)
        {
            Task<AccountWithdrawResponseDto> response = Policy.HandleResult<AccountWithdrawResponseDto>(x => x.Amount == 0)
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(30), (_, timeSpan, retryCount, _) => Console.WriteLine($"Retry : {retryCount}, Waiting : {timeSpan} "))
            .ExecuteAsync(async () => await _accountService.WithdrawAsync(model, cancellationToken));

            return ResponseDto.Success(response, response.Result.Amount != 0, "İşlem yapılırken bir hata meydana geldi");
        }

        [HttpPost("{id}/withdraw")]
        [MapToApiVersion("2")]
        public async Task<ActionResult<ResponseDto>> Withdraw2([FromBody] AccountWithdrawRequestDto model, CancellationToken cancellationToken)
        {
            model.Amount *= 2; //For understand that different version
            Task<AccountWithdrawResponseDto> response = Policy.HandleResult<AccountWithdrawResponseDto>(x => x.Amount == 0)
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(30), (_, timeSpan, retryCount, _) => Console.WriteLine($"Retry : {retryCount}, Waiting : {timeSpan} "))
            .ExecuteAsync(async () => await _accountService.WithdrawAsync(model, cancellationToken));

            return ResponseDto.Success(response, response.Result.Amount != 0, "İşlem yapılırken bir hata meydana geldi");
        }

        [HttpGet("{id}/balance")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<ActionResult<ResponseDto>> Balance([FromBody] AccountBalanceRequestDto model, CancellationToken cancellationToken)
        {
            Task<AccountBalanceResponseDto> response = Policy.HandleResult<AccountBalanceResponseDto>(x => !x.Items.Any())
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(30), (_, timeSpan, retryCount, _) => Console.WriteLine($"Retry : {retryCount}, Waiting : {timeSpan} "))
            .ExecuteAsync(async () => await _accountService.BalanceAsync(model, cancellationToken));

            return ResponseDto.Success(response, response.Result.Items.Any(), "İşlem yapılırken bir hata meydana geldi");
        }
    }
}