using crypto.Models;

namespace crypto.Interfaces
{
    public interface ITradeService
    {
        Task<Transaction?> BuyCryptoAsync(int userId, int cryptoId, decimal amount);
        Task<Transaction?> SellCryptoAsync(int userId, int cryptoId, decimal amount);
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
        Task<Transaction?> GetTransactionDetailsAsync(int transactionId);
        Task<dynamic> CalculateUserProfitAsync(int userId);
        Task<dynamic> GetDetailedProfitAsync(int userId);
    }
}