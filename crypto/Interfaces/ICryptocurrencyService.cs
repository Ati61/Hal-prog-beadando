using crypto.Models;

namespace crypto.Interfaces
{
    public interface ICryptocurrencyService
    {
        Task<IEnumerable<Cryptocurrency>> GetAllCryptocurrenciesAsync();
        Task<Cryptocurrency?> GetCryptocurrencyByIdAsync(int id);
        Task<Cryptocurrency> AddCryptocurrencyAsync(Cryptocurrency cryptocurrency);
        Task<bool> DeleteCryptocurrencyAsync(int id);
        Task<Cryptocurrency?> UpdateCryptoPriceAsync(int id, decimal newPrice);
        Task<IEnumerable<PriceHistory>> GetPriceHistoryAsync(int cryptoId);
        Task SeedInitialCryptocurrenciesAsync();
    }
}