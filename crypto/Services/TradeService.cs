using crypto.Interfaces;
using crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace crypto.Services
{
    public class TradeService : ITradeService
    {
        private readonly CryptoDbContext _context;

        public TradeService(CryptoDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> BuyCryptoAsync(int userId, int cryptoId, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return null;
                }

                var crypto = await _context.Cryptocurrencies.FindAsync(cryptoId);
                if (crypto == null)
                {
                    return null;
                }

                // Calculate total cost
                decimal totalCost = amount * crypto.CurrentPrice;

                // Check if user has enough balance
                if (user.Wallet.Balance < totalCost)
                {
                    throw new InvalidOperationException("Insufficient funds");
                }

                // Update wallet balance
                user.Wallet.Balance -= totalCost;

                // Check if user already owns this crypto
                var walletCrypto = await _context.WalletCryptos
                    .FirstOrDefaultAsync(wc => wc.WalletId == user.Wallet.Id && wc.CryptocurrencyId == cryptoId);

                if (walletCrypto == null)
                {
                    // User doesn't own this crypto yet, create new wallet crypto
                    walletCrypto = new WalletCrypto
                    {
                        WalletId = user.Wallet.Id,
                        CryptocurrencyId = cryptoId,
                        Amount = amount,
                        AverageAcquisitionPrice = crypto.CurrentPrice
                    };
                    _context.WalletCryptos.Add(walletCrypto);
                }
                else
                {
                    // User already owns this crypto, update amount and average acquisition price
                    decimal totalValue = (walletCrypto.Amount * walletCrypto.AverageAcquisitionPrice) + totalCost;
                    walletCrypto.Amount += amount;
                    walletCrypto.AverageAcquisitionPrice = totalValue / walletCrypto.Amount;
                }

                // Create transaction record
                var trx = new Transaction
                {
                    UserId = userId,
                    CryptocurrencyId = cryptoId,
                    Type = "BUY",
                    Amount = amount,
                    Price = crypto.CurrentPrice,
                    TotalValue = totalCost,
                    Timestamp = DateTime.UtcNow
                };
                _context.Transactions.Add(trx);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return trx;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Transaction?> SellCryptoAsync(int userId, int cryptoId, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return null;
                }

                var crypto = await _context.Cryptocurrencies.FindAsync(cryptoId);
                if (crypto == null)
                {
                    return null;
                }

                // Check if user owns this crypto
                var walletCrypto = await _context.WalletCryptos
                    .FirstOrDefaultAsync(wc => wc.WalletId == user.Wallet.Id && wc.CryptocurrencyId == cryptoId);

                if (walletCrypto == null || walletCrypto.Amount < amount)
                {
                    throw new InvalidOperationException("Insufficient crypto balance");
                }

                // Calculate total value
                decimal totalValue = amount * crypto.CurrentPrice;

                // Update wallet balance
                user.Wallet.Balance += totalValue;

                // Update wallet crypto
                walletCrypto.Amount -= amount;
                if (walletCrypto.Amount <= 0)
                {
                    // If no more crypto left, remove the wallet crypto entry
                    _context.WalletCryptos.Remove(walletCrypto);
                }

                // Create transaction record
                var trx = new Transaction
                {
                    UserId = userId,
                    CryptocurrencyId = cryptoId,
                    Type = "SELL",
                    Amount = amount,
                    Price = crypto.CurrentPrice,
                    TotalValue = totalValue,
                    Timestamp = DateTime.UtcNow
                };
                _context.Transactions.Add(trx);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return trx;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId)
        {
            return await _context.Transactions
                .Include(t => t.Cryptocurrency)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionDetailsAsync(int transactionId)
        {
            return await _context.Transactions
                .Include(t => t.Cryptocurrency)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<dynamic> CalculateUserProfitAsync(int userId)
        {
            decimal totalProfit = 0;
            
            var walletCryptos = await _context.WalletCryptos
                .Include(wc => wc.Cryptocurrency)
                .Where(wc => wc.Wallet.UserId == userId)
                .ToListAsync();

            foreach (var walletCrypto in walletCryptos)
            {
                decimal currentValue = walletCrypto.Amount * walletCrypto.Cryptocurrency.CurrentPrice;
                decimal acquisitionValue = walletCrypto.Amount * walletCrypto.AverageAcquisitionPrice;
                decimal profit = currentValue - acquisitionValue;
                totalProfit += profit;
            }

            return new { TotalProfit = totalProfit };
        }

        public async Task<dynamic> GetDetailedProfitAsync(int userId)
        {
            var walletCryptos = await _context.WalletCryptos
                .Include(wc => wc.Cryptocurrency)
                .Where(wc => wc.Wallet.UserId == userId)
                .ToListAsync();

            var result = new List<object>();
            decimal totalProfit = 0;

            foreach (var walletCrypto in walletCryptos)
            {
                decimal currentValue = walletCrypto.Amount * walletCrypto.Cryptocurrency.CurrentPrice;
                decimal acquisitionValue = walletCrypto.Amount * walletCrypto.AverageAcquisitionPrice;
                decimal profit = currentValue - acquisitionValue;
                totalProfit += profit;

                result.Add(new
                {
                    CryptocurrencyId = walletCrypto.CryptocurrencyId,
                    Symbol = walletCrypto.Cryptocurrency.Symbol,
                    Name = walletCrypto.Cryptocurrency.Name,
                    Amount = walletCrypto.Amount,
                    AverageAcquisitionPrice = walletCrypto.AverageAcquisitionPrice,
                    CurrentPrice = walletCrypto.Cryptocurrency.CurrentPrice,
                    AcquisitionValue = acquisitionValue,
                    CurrentValue = currentValue,
                    Profit = profit,
                    ProfitPercentage = acquisitionValue > 0 ? (profit / acquisitionValue) * 100 : 0
                });
            }

            return new
            {
                DetailedProfits = result,
                TotalProfit = totalProfit
            };
        }
    }
}