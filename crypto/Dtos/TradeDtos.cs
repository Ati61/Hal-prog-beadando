using System.ComponentModel.DataAnnotations;

namespace crypto.Dtos
{
    public class TradeRequestDto 
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int CryptoId { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }

    public class TransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CryptocurrencyId { get; set; }
        public string CryptocurrencySymbol { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "BUY" v "SELL"
        public decimal Amount { get; set; }
        public decimal Price { get; set; } // vásárlási ár
        public decimal TotalValue { get; set; } // összesen
        public DateTime Timestamp { get; set; }
    }
}
