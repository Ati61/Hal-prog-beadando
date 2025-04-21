using System.ComponentModel.DataAnnotations;

namespace crypto.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CryptocurrencyId { get; set; }
        public DateTime Timestamp { get; set; }
        [Required]
        [RegularExpression("BUY|SELL", ErrorMessage = "Type must be either 'BUY' or 'SELL'")]
        public string Type { get; set; } = string.Empty; // "BUY" or "SELL"
        [Range(0.00000001, double.MaxValue)]
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public decimal TotalValue { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Cryptocurrency Cryptocurrency { get; set; } = null!;
    }
}
