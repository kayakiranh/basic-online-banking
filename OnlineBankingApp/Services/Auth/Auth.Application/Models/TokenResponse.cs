using System.ComponentModel.DataAnnotations;

namespace Auth.Application.Models
{
    [Serializable]
    public class TokenResponse
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime TokenExpireDate { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}