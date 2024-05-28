using System.ComponentModel.DataAnnotations;

namespace Auth.Application.Models
{
    [Serializable]
    public record TokenRequest
    {
        [Required]
        public required string IdentityNumber { get; set; }
    }
}