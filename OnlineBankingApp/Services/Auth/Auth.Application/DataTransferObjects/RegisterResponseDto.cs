using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DataTransferObjects
{
    [Serializable]
    public record RegisterResponseDto
    {
        [Required]
        public required string Message { get; set; } = string.Empty;

        [Required]
        public required bool Result { get; set; }

        public static RegisterResponseDto ValidateError(string message)
        {
            return new RegisterResponseDto
            {
                Result = false,
                Message = message,
            };
        }
    }
}