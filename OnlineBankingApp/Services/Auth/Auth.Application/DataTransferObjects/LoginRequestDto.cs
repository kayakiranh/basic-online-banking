using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DataTransferObjects
{
    [Serializable]
    public record LoginRequestDto
    {
        [Required(ErrorMessage = "TC kimlik no alanı boş geçilemez")]
        [MinLength(11, ErrorMessage = "TC kimlik no alanı 11 hane olmalıdır")]
        [MaxLength(11, ErrorMessage = "TC kimlik no alanı 11 hane olmalıdır")]
        public required string IdentityNumber { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş geçilemez")]
        public required string Password { get; set; }
    }
}