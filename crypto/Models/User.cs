using System.ComponentModel.DataAnnotations;

namespace crypto.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; 

        // Navigation property
        public Wallet Wallet { get; set; } = new Wallet();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
