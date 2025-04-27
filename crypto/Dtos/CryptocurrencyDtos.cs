using System.ComponentModel.DataAnnotations;

namespace crypto.Dtos
{
    public class CryptocurrencyDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
    }

    public class CryptocurrencyCreateDto
    {
        [Required]
        public string Symbol { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public decimal CurrentPrice { get; set; }
    }

    public class PriceUpdateDto 
    {
        [Required]
        public int CryptoId { get; set; }
        [Required]
        public decimal NewPrice { get; set; }
    }

     public class PriceHistoryDto
    {
        public int Id { get; set; }
        public int CryptocurrencyId { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
