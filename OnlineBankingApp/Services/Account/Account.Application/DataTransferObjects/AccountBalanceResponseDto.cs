using System.ComponentModel.DataAnnotations;

namespace Account.Application.DataTransferObjects
{
    [Serializable]
    public class AccountBalanceResponseDto
    {
        [Required]
        public List<AccountBalanceResponseItemDto> Items { get; set; } = new List<AccountBalanceResponseItemDto>();

        public static AccountBalanceResponseDto ValidateError()
        {
            return new AccountBalanceResponseDto
            {
                Items = new List<AccountBalanceResponseItemDto>()
            };
        }
    }

    [Serializable]
    public class AccountBalanceResponseItemDto
    {
        [Required]
        public required string AccountNumber { get; set; }

        [Required]
        public required string From { get; set; }

        [Required]
        public required string To { get; set; }

        [Required]
        public decimal Amount { get; set; } = 0;
    }
}