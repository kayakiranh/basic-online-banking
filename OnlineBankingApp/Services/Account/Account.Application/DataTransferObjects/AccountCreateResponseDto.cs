using System.ComponentModel.DataAnnotations;

namespace Account.Application.DataTransferObjects
{
    [Serializable]
    public class AccountCreateResponseDto
    {
        [Required]
        public required string AccountNumber { get; set; }

        [Required]
        public required string AccountHolderName { get; set; }

        [Required]
        public required decimal Balance { get; set; } = 0;

        public static AccountCreateResponseDto ValidateError(string message)
        {
            return new AccountCreateResponseDto
            {
                AccountNumber = string.Empty,
                AccountHolderName = string.Empty,
                Balance = 0
            };
        }
    }
}