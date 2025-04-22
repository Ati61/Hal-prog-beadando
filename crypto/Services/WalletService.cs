using crypto.Interfaces;
using crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace crypto.Services
{
    public class WalletService : IWalletService
    {
        private readonly CryptoDbContext _context;

        public WalletService(CryptoDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetWalletByUserIdAsync(int userId)
        {
            return await _context.Wallets
                .Include(w => w.WalletCryptos)
                .ThenInclude(wc => wc.Cryptocurrency)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<Wallet?> UpdateWalletBalanceAsync(int userId, decimal newBalance)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                return null;
            }

            wallet.Balance = newBalance;
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<bool> DeleteWalletAsync(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                return false;
            }

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WalletCrypto>> GetUserCryptosAsync(int userId)
        {
            var wallet = await _context.Wallets
                .Include(w => w.WalletCryptos)
                .ThenInclude(wc => wc.Cryptocurrency)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            return wallet?.WalletCryptos ?? new List<WalletCrypto>();
        }
    }
}