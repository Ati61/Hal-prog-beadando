namespace crypto.Models
{
    public class PriceHistory
    {
        public int Id { get; set; }
        public int CryptocurrencyId { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }

        // Navigation property
        public Cryptocurrency Cryptocurrency { get; set; } = null!;
    }
}
