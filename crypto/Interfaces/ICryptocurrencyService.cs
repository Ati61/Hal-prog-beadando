using crypto.Models;
using crypto.Dtos; 
using System.Collections.Generic; 

namespace crypto.Interfaces
{
    public interface ICryptocurrencyService
    {
        Task<IEnumerable<CryptocurrencyDto>> GetAllCryptocurrenciesAsync(); 
        Task<CryptocurrencyDto?> GetCryptocurrencyByIdAsync(int id);
        Task<CryptocurrencyDto> AddCryptocurrencyAsync(CryptocurrencyCreateDto cryptocurrencyDto);
        Task<bool> DeleteCryptocurrencyAsync(int id);
        Task<CryptocurrencyDto?> UpdateCryptoPriceAsync(int id, decimal newPrice); 
        Task<IEnumerable<PriceHistoryDto>> GetPriceHistoryAsync(int cryptoId); 
        Task SeedInitialCryptocurrenciesAsync();
        Task UpdateAllPricesRandomlyAsync();
    }
}