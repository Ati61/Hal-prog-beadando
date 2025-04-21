namespace crypto.Models
{
    public class WalletCrypto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public int CryptocurrencyId { get; set; }
        public decimal Amount { get; set; }
        public decimal AverageAcquisitionPrice { get; set; }

        // Navigation properties
        public Wallet Wallet { get; set; } = null!;
        public Cryptocurrency Cryptocurrency { get; set; } = null!;
    }
}
