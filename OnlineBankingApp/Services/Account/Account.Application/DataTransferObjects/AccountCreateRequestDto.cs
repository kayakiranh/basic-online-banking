using System.ComponentModel.DataAnnotations;

namespace Account.Application.DataTransferObjects
{
    [Serializable]
    public class AccountCreateRequestDto
    {
        [Required]
        [MaxLength(29)]
        [MinLength(29)]
        public required string AccountNumber { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        public required string AccountHolderName { get; set; }

        [Required]
        public required decimal Balance { get; set; } = 0;
    }
}