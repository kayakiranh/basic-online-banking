using System.ComponentModel.DataAnnotations;

namespace Account.Application.DataTransferObjects
{
    [Serializable]
    public class AccountDepositResponseDto
    {
        [Required]
        [MaxLength(29)]
        [MinLength(29)]
        public required string AccountNumber { get; set; }

        [Required]
        [MaxLength(29)]
        [MinLength(29)]
        public required string From { get; set; }

        [Required]
        [Range(0.01, 99999999)]
        public required decimal Amount { get; set; }

        public static AccountDepositResponseDto ValidateError(string message)
        {
            return new AccountDepositResponseDto
            {
                AccountNumber = string.Empty,
                From = string.Empty,
                Amount = 0
            };
        }
    }
}