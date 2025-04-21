namespace crypto.Models
{
    public class Cryptocurrency
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;// BTC, ETH, etc.
        public string Name { get; set; } = string.Empty; // Bitcoin, Ethereum, etc.
        public decimal CurrentPrice { get; set; }

        // Navigation properties
        public ICollection<WalletCrypto> WalletCryptos { get; set; } = new List<WalletCrypto>();
        public ICollection<PriceHistory> PriceHistory { get; set; } = new List<PriceHistory>();
    }
}
