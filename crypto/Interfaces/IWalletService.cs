using crypto.Models;

namespace crypto.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet?> GetWalletByUserIdAsync(int userId);
        Task<Wallet?> UpdateWalletBalanceAsync(int userId, decimal newBalance);
        Task<bool> DeleteWalletAsync(int userId);
        Task<IEnumerable<WalletCrypto>> GetUserCryptosAsync(int userId);
    }
}