using System.ComponentModel.DataAnnotations;

namespace Account.Application.DataTransferObjects
{
    [Serializable]
    public class AccountDepositRequestDto
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
    }
}