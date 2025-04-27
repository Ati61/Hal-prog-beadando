using System.Collections.Generic;

namespace crypto.Dtos
{
    public class TotalProfitDto
    {
        public decimal TotalProfit { get; set; }
    }

    public class DetailedProfitDto
    {
        public List<CryptoProfitDetailDto> DetailedProfits { get; set; } = new List<CryptoProfitDetailDto>();
        public decimal TotalProfit { get; set; }
    }

    public class CryptoProfitDetailDto
    {
        public int CryptocurrencyId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal AverageAcquisitionPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal AcquisitionValue { get; set; } // Amount * AverageAcquisitionPrice
        public decimal CurrentValue { get; set; } // Amount * CurrentPrice
        public decimal Profit { get; set; } // CurrentValue - AcquisitionValue
        public decimal ProfitPercentage { get; set; } // (Profit / AcquisitionValue) * 100
    }
}
