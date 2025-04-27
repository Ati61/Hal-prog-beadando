using crypto.Models;
using crypto.Dtos;
using System.Collections.Generic; 

namespace crypto.Interfaces
{
    public interface IWalletService
    {
        Task<WalletDto?> GetWalletByUserIdAsync(int userId); 
        Task<WalletDto?> UpdateWalletBalanceAsync(int userId, decimal newBalance); 
        Task<bool> DeleteWalletAsync(int userId);
        Task<IEnumerable<WalletCryptoDto>> GetUserCryptosAsync(int userId); 
    }
}