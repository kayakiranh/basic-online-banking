using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DataTransferObjects
{
    [Serializable]
    public record RegisterRequestDto
    {
        [Required(ErrorMessage = "Ad alanı boş geçilemez")]
        [MaxLength(30, ErrorMessage = "Ad alanı en fazla 30 karakter olmalıdır")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı boş geçilemez")]
        [MaxLength(30, ErrorMessage = "Soyad alanı en fazla 30 karakter olmalıdır")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "TC kimlik no alanı boş geçilemez")]
        [MinLength(11, ErrorMessage = "TC kimlik no alanı 11 hane olmalıdır")]
        [MaxLength(11, ErrorMessage = "TC kimlik no alanı 11 hane olmalıdır")]
        public required string IdentityNumber { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş geçilemez")]
        public required string Password { get; set; }
    }
}