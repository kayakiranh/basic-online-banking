using Account.Application.DataTransferObjects;
using Account.Application.Interfaces;
using Account.Core.Domain;
using Helper.Infrastructure;
using Logging.Infrastructure;
using Mailing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PdfGenerator.Infrastructure;
using AccountOwner = Account.Core.Domain.Account;

namespace Account.Infrastructure.Services
{
    [Serializable]
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountOpenReceiver _accountOpenReceiver;
        private readonly IConfiguration _configuration;
        private readonly ILoggingService _loggingService;
        private readonly IMailingService _mailingService;
        private readonly IPdfService _pdfService;
        private readonly string _queueName;

        public AccountService(ApplicationDbContext context, IAccountOpenReceiver accountOpenReceiver, IConfiguration configuration, ILoggingService loggingService, IMailingService mailingService, IPdfService pdfService)
        {
            _context = context;
            _accountOpenReceiver = accountOpenReceiver;
            _configuration = configuration;
            _loggingService = loggingService;
            _mailingService = mailingService;
            _pdfService = pdfService;
            _queueName = _configuration.GetSection("RabbitMq:QueueName").Value;
        }

        public async Task<AccountCreateResponseDto> CreateAsync(AccountCreateRequestDto request, CancellationToken cancellationToken)
        {
            if (!request.AccountNumber.Validator(29, 29) || !request.AccountHolderName.Validator(5, 50))
            {
                _loggingService.Information("Giriş alanlarını kontrol ediniz", request);
                return AccountCreateResponseDto.ValidateError("Giriş alanlarını kontrol ediniz");
            }

            bool checkAccountByAccountNumber = await _context.Accounts.AnyAsync(u => u.AccountNumber == request.AccountNumber);
            if (checkAccountByAccountNumber)
            {
                _loggingService.Information("Daha hesap zaten kayıtlı", request);
                return AccountCreateResponseDto.ValidateError("Daha hesap zaten kayıtlı");
            }

            AccountOwner account = new AccountOwner
            {
                AccountHolderName = request.AccountHolderName,
                AccountNumber = request.AccountNumber,
                Balance = 0,
                CreatedDate = DateTime.Now
            };

            AccountTransaction accountTransaction = new AccountTransaction
            {
                AccountNumber = request.AccountNumber,
                Amount = 0,
                CreatedDate = DateTime.Now,
                From = "Bank",
                To = request.AccountNumber
            };

            await _context.Accounts.AddAsync(account, cancellationToken);
            await _context.AccountTransactions.AddAsync(accountTransaction, cancellationToken);
            int dbResult = await _context.SaveChangesAsync(cancellationToken);

            if (dbResult < 1)
            {
                _loggingService.Warning("İşlem yapılırken bir hata meydana geldi", request);
                return AccountCreateResponseDto.ValidateError("İşlem yapılırken bir hata meydana geldi");
            }

            AccountCreateResponseDto accountCreateResponseDto = new AccountCreateResponseDto
            {
                AccountHolderName = account.AccountHolderName,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance
            };

            _accountOpenReceiver.OpenAccount(accountCreateResponseDto);
            _mailingService.Send($"Hesabınız oluşturuldu : {accountCreateResponseDto.AccountNumber}");

            return accountCreateResponseDto;
        }

        public async Task<AccountBalanceResponseDto> BalanceAsync(AccountBalanceRequestDto request, CancellationToken cancellationToken)
        {
            if (!request.AccountNumber.Validator(29, 29))
            {
                _loggingService.Information("Giriş alanlarını kontrol ediniz", request);
                return AccountBalanceResponseDto.ValidateError();
            }

            List<AccountTransaction> accountTransactions = await _context.AccountTransactions.Where(u => u.AccountNumber == request.AccountNumber).ToListAsync(cancellationToken);
            if (accountTransactions.Count == 0)
            {
                _loggingService.Information("Kullanıcı bilgileriniz eşleşmiyor", request);
                return AccountBalanceResponseDto.ValidateError();
            }

            AccountBalanceResponseDto accountBalanceResponseDto = new AccountBalanceResponseDto { Items = new List<AccountBalanceResponseItemDto>() };
            accountTransactions.ForEach(x => accountBalanceResponseDto.Items.Add(new AccountBalanceResponseItemDto { AccountNumber = x.AccountNumber, From = x.From, Amount = x.Amount, To = x.To }));

            string pdfPath = _pdfService.Generate(request.AccountNumber);
            _mailingService.Send($"Hesap özetiniz", pdfPath);

            return accountBalanceResponseDto;
        }

        public async Task<AccountDepositResponseDto> DepositAsync(AccountDepositRequestDto request, CancellationToken cancellationToken)
        {
            if (
                !request.AccountNumber.Validator(29, 29) ||
                !request.From.Validator(29, 29) ||
                !request.Amount.GreaterThanZero()
                )
            {
                _loggingService.Information("Giriş alanlarını kontrol ediniz", request);
                return AccountDepositResponseDto.ValidateError("Giriş alanlarını kontrol ediniz");
            }

            AccountOwner? senderAccount = await _context.Accounts.FirstOrDefaultAsync(u => u.AccountNumber == request.From);
            if (senderAccount == null)
            {
                _loggingService.Information("Gönderici hesap bulunamadı", request);
                return AccountDepositResponseDto.ValidateError("Gönderici hesap bulunamadı");
            }
            if (senderAccount.Balance < request.Amount)
            {
                _loggingService.Information("Yetersiz bakiye", request);
                return AccountDepositResponseDto.ValidateError("Yetersiz bakiye");
            }
            AccountOwner? receiverAccount = await _context.Accounts.FirstOrDefaultAsync(u => u.AccountNumber == request.AccountNumber);
            if (receiverAccount == null)
            {
                _loggingService.Information("Alıcı hesap bulunamadı", request);
                return AccountDepositResponseDto.ValidateError("Alıcı hesap bulunamadı");
            }

            senderAccount.Balance -= request.Amount;
            _context.Accounts.Update(senderAccount);

            receiverAccount.Balance += request.Amount;
            _context.Accounts.Update(receiverAccount);

            AccountTransaction accountTransaction = new AccountTransaction
            {
                AccountNumber = request.AccountNumber,
                Amount = request.Amount,
                CreatedDate = DateTime.Now,
                From = receiverAccount.AccountNumber,
                To = request.AccountNumber
            };
            await _context.AccountTransactions.AddAsync(accountTransaction, cancellationToken);
            int dbResult = await _context.SaveChangesAsync(cancellationToken);

            if (dbResult < 1)
            {
                _loggingService.Warning("İşlem yapılırken bir hata meydana geldi", request);
                return AccountDepositResponseDto.ValidateError("İşlem yapılırken bir hata meydana geldi");
            }

            AccountDepositResponseDto accountDepositResponseDto = new AccountDepositResponseDto
            {
                AccountNumber = request.AccountNumber,
                Amount = request.Amount,
                From = request.From
            };

            string pdfPath = _pdfService.Generate(request.AccountNumber);
            _mailingService.Send($"Hesabınıza para geldi", pdfPath);

            return accountDepositResponseDto;
        }

        public async Task<AccountWithdrawResponseDto> WithdrawAsync(AccountWithdrawRequestDto request, CancellationToken cancellationToken)
        {
            if (
                !request.AccountNumber.Validator(29, 29) ||
                !request.From.Validator(29, 29) ||
                !request.Amount.GreaterThanZero()
                )
            {
                _loggingService.Information("Giriş alanlarını kontrol ediniz", request);
                return AccountWithdrawResponseDto.ValidateError("Giriş alanlarını kontrol ediniz");
            }

            AccountOwner? senderAccount = await _context.Accounts.FirstOrDefaultAsync(u => u.AccountNumber == request.From);
            if (senderAccount == null)
            {
                _loggingService.Information("Gönderici hesap bulunamadı", request);
                return AccountWithdrawResponseDto.ValidateError("Gönderici hesap bulunamadı");
            }
            if (senderAccount.Balance < request.Amount)
            {
                _loggingService.Information("Yetersiz bakiye", request);
                return AccountWithdrawResponseDto.ValidateError("Yetersiz bakiye");
            }
            AccountOwner? receiverAccount = await _context.Accounts.FirstOrDefaultAsync(u => u.AccountNumber == request.AccountNumber);
            if (receiverAccount == null)
            {
                _loggingService.Information("Alıcı hesap bulunamadı", request);
                return AccountWithdrawResponseDto.ValidateError("Alıcı hesap bulunamadı");
            }

            senderAccount.Balance -= request.Amount;
            _context.Accounts.Update(senderAccount);

            receiverAccount.Balance += request.Amount;
            _context.Accounts.Update(receiverAccount);

            AccountTransaction accountTransaction = new AccountTransaction
            {
                AccountNumber = request.AccountNumber,
                Amount = request.Amount,
                CreatedDate = DateTime.Now,
                From = receiverAccount.AccountNumber,
                To = request.AccountNumber
            };
            await _context.AccountTransactions.AddAsync(accountTransaction, cancellationToken);
            int dbResult = await _context.SaveChangesAsync(cancellationToken);

            if (dbResult < 1)
            {
                _loggingService.Warning("İşlem yapılırken bir hata meydana geldi", request);
                return AccountWithdrawResponseDto.ValidateError("İşlem yapılırken bir hata meydana geldi");
            }

            AccountWithdrawResponseDto accountDepositResponseDto = new AccountWithdrawResponseDto
            {
                AccountNumber = request.AccountNumber,
                Amount = request.Amount,
                From = request.From
            };

            string pdfPath = _pdfService.Generate(request.From);
            _mailingService.Send($"Hesabınızdan para çekildi", pdfPath);

            return accountDepositResponseDto;
        }
    }
}