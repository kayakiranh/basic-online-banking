using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Core.Domain
{
    [Serializable]
    [Table("Accounts")]
    public class Account
    {
        //1,2,3...
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[TR] ülke kodu
        //[00] kontrol kodu
        //[00000] banka kodu (5 hane)
        //[0] rezerv alan
        //[0000000000000000] hesap no (16 hane)
        //TR00 00000 0 0000000000000000 (29 hane)
        [Required]
        [MaxLength(29)]
        [MinLength(29)]
        public required string AccountNumber { get; set; }

        //Hüseyin Kayakıran
        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        public required string AccountHolderName { get; set; }

        //Hesap ilk açıldığında 0
        [Required]
        public required decimal Balance { get; set; } = 0;

        //Hesap ilk açıldığındaki tarih
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}