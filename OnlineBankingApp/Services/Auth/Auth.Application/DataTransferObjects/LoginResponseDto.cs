using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DataTransferObjects
{
    [Serializable]
    public record LoginResponseDto
    {
        [Required]
        public required string Message { get; set; } = string.Empty;

        [Required]
        public required bool Result { get; set; }

        [Required]
        public required string AccessToken { get; set; }

        [Required]
        public required string RefreshToken { get; set; }

        [Required]
        public required DateTime AccessTokenExpireDate { get; set; }

        public static LoginResponseDto ValidateError(string message)
        {
            return new LoginResponseDto
            {
                AccessTokenExpireDate = DateTime.Now,
                Result = false,
                AccessToken = string.Empty,
                Message = message,
                RefreshToken = string.Empty
            };
        }
    }
}