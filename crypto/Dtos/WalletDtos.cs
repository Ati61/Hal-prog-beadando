using System.Collections.Generic;

namespace crypto.Dtos
{
    public class WalletDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public List<WalletCryptoDto> Cryptos { get; set; } = new List<WalletCryptoDto>();
    }

    public class WalletCryptoDto
    {
        public int CryptocurrencyId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal AverageAcquisitionPrice { get; set; }
        public decimal CurrentPrice { get; set; } 
        public decimal CurrentValue { get; set; }
    }

    public class UpdateBalanceDto
    {
        public decimal NewBalance { get; set; }
    }
}
