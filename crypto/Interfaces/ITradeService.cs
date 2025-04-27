using crypto.Models;
using crypto.Dtos; 
using System.Collections.Generic; 

namespace crypto.Interfaces
{
    public interface ITradeService
    {
        Task<TransactionDto?> BuyCryptoAsync(int userId, int cryptoId, decimal amount); 
        Task<TransactionDto?> SellCryptoAsync(int userId, int cryptoId, decimal amount); 
        Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId);
        Task<TransactionDto?> GetTransactionDetailsAsync(int transactionId); 
        Task<TotalProfitDto> CalculateUserProfitAsync(int userId);
        Task<DetailedProfitDto> GetDetailedProfitAsync(int userId);
    }
}