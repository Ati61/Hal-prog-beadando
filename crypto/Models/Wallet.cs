namespace crypto.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; } // In USD or other base currency

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<WalletCrypto> WalletCryptos { get; set; } = new List<WalletCrypto>();
    }
}
