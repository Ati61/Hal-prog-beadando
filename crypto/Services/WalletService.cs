using crypto.Interfaces;
using crypto.Models;
using crypto.Dtos; 
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; 
using System.Linq; 

namespace crypto.Services
{
    public class WalletService : IWalletService
    {
        private readonly CryptoDbContext _context;

        public WalletService(CryptoDbContext context)
        {
            _context = context;
        }

        public async Task<WalletDto?> GetWalletByUserIdAsync(int userId)
        {
            var wallet = await _context.Wallets
                .Include(w => w.WalletCryptos)
                .ThenInclude(wc => wc.Cryptocurrency)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null) return null;

            return new WalletDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                Cryptos = wallet.WalletCryptos.Select(wc => new WalletCryptoDto
                {
                    CryptocurrencyId = wc.CryptocurrencyId,
                    Symbol = wc.Cryptocurrency.Symbol,
                    Name = wc.Cryptocurrency.Name,
                    Amount = wc.Amount,
                    AverageAcquisitionPrice = wc.AverageAcquisitionPrice,
                    CurrentPrice = wc.Cryptocurrency.CurrentPrice, 
                    CurrentValue = wc.Amount * wc.Cryptocurrency.CurrentPrice 
                }).ToList()
            };
        }

        public async Task<WalletDto?> UpdateWalletBalanceAsync(int userId, decimal newBalance)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                return null;
            }

            wallet.Balance = newBalance;
            await _context.SaveChangesAsync();

     
            return new WalletDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                Cryptos = new List<WalletCryptoDto>() 
            };
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

        public async Task<IEnumerable<WalletCryptoDto>> GetUserCryptosAsync(int userId)
        {
            var wallet = await _context.Wallets
                .Include(w => w.WalletCryptos)
                .ThenInclude(wc => wc.Cryptocurrency)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null) return new List<WalletCryptoDto>(); 

            return wallet.WalletCryptos.Select(wc => new WalletCryptoDto
            {
                CryptocurrencyId = wc.CryptocurrencyId,
                Symbol = wc.Cryptocurrency.Symbol,
                Name = wc.Cryptocurrency.Name,
                Amount = wc.Amount,
                AverageAcquisitionPrice = wc.AverageAcquisitionPrice,
                CurrentPrice = wc.Cryptocurrency.CurrentPrice,
                CurrentValue = wc.Amount * wc.Cryptocurrency.CurrentPrice
            }).ToList();
        }
    }
}