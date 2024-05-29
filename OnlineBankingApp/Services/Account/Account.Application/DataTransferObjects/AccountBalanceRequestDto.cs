using System.ComponentModel.DataAnnotations;

namespace Account.Application.DataTransferObjects
{
    [Serializable]
    public class AccountBalanceRequestDto
    {
        [Required]
        [MaxLength(29)]
        [MinLength(29)]
        public required string AccountNumber { get; set; }
    }
}