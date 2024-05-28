using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Core.Domain
{
    [Serializable]
    [Table("Users")]
    public class User
    {
        //1,2,3...
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Hüseyin
        [Required]
        [MaxLength(30)]
        [MinLength(1)]
        public string FirstName { get; set; }

        //Kayakıran
        [Required]
        [MaxLength(30)]
        [MinLength(1)]
        public string LastName { get; set; }

        //TC kimlik no
        [Required]
        [MaxLength(11)]
        [MinLength(11)]
        public string IdentityNumber { get; set; }

        //Salt + Hash
        [Required]
        [MaxLength(250)]
        public string Password { get; set; }

        //Token
        [Required]
        public string Token { get; set; }

        //RefreshToken
        [Required]
        public string RefreshToken { get; set; }

        [Required]
        public DateTime Created { get; set; } = DateTime.Now;
    }
}