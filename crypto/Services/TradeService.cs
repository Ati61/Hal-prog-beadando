using crypto.Interfaces;
using crypto.Models;
using crypto.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; 
using System.Linq; 

namespace crypto.Services
{
    public class TradeService : ITradeService
    {
        private readonly CryptoDbContext _context;

        public TradeService(CryptoDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionDto?> BuyCryptoAsync(int userId, int cryptoId, decimal amount) 
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync(); 
            try
            {
                var user = await _context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null || user.Wallet == null) 
                {
                    return null; 
                }

                var crypto = await _context.Cryptocurrencies.FindAsync(cryptoId);
                if (crypto == null)
                {
                    return null; 
                }

                decimal totalCost = amount * crypto.CurrentPrice;
                if (user.Wallet.Balance < totalCost)
                {
                    throw new InvalidOperationException("Insufficient funds");
                }

                user.Wallet.Balance -= totalCost;

                var walletCrypto = await _context.WalletCryptos
                    .FirstOrDefaultAsync(wc => wc.WalletId == user.Wallet.Id && wc.CryptocurrencyId == cryptoId);

                if (walletCrypto == null)
                {
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
                    decimal existingValue = walletCrypto.Amount * walletCrypto.AverageAcquisitionPrice;
                    decimal newValue = totalCost;
                    walletCrypto.Amount += amount;
                    //0val nem osztunk 
                    walletCrypto.AverageAcquisitionPrice = walletCrypto.Amount > 0 ? (existingValue + newValue) / walletCrypto.Amount : 0;
                }

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
                await dbTransaction.CommitAsync();

                return new TransactionDto
                {
                    Id = trx.Id,
                    UserId = trx.UserId,
                    CryptocurrencyId = trx.CryptocurrencyId,
                    CryptocurrencySymbol = crypto.Symbol,
                    Type = trx.Type,
                    Amount = trx.Amount,
                    Price = trx.Price,
                    TotalValue = trx.TotalValue,
                    Timestamp = trx.Timestamp
                };
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<TransactionDto?> SellCryptoAsync(int userId, int cryptoId, decimal amount)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync(); 
            try
            {
                var user = await _context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null || user.Wallet == null)
                {
                    return null;
                }

                var crypto = await _context.Cryptocurrencies.FindAsync(cryptoId);
                if (crypto == null)
                {
                    return null;
                }

                var walletCrypto = await _context.WalletCryptos
                    .FirstOrDefaultAsync(wc => wc.WalletId == user.Wallet.Id && wc.CryptocurrencyId == cryptoId);

                if (walletCrypto == null || walletCrypto.Amount < amount)
                {
                    throw new InvalidOperationException("Insufficient crypto balance");
                }

                decimal totalValue = amount * crypto.CurrentPrice;
                user.Wallet.Balance += totalValue;
                walletCrypto.Amount -= amount;

                if (walletCrypto.Amount <= 0)
                {
                    _context.WalletCryptos.Remove(walletCrypto);
                }
                // Note: AverageAcquisitionPrice nem valtozunk eladáskor

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
                await dbTransaction.CommitAsync();


                return new TransactionDto
                {
                    Id = trx.Id,
                    UserId = trx.UserId,
                    CryptocurrencyId = trx.CryptocurrencyId,
                    CryptocurrencySymbol = crypto.Symbol, 
                    Type = trx.Type,
                    Amount = trx.Amount,
                    Price = trx.Price,
                    TotalValue = trx.TotalValue,
                    Timestamp = trx.Timestamp
                };
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId) 
        {
            return await _context.Transactions
                .Include(t => t.Cryptocurrency) 
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Timestamp)
                .Select(trx => new TransactionDto
                {
                    Id = trx.Id,
                    UserId = trx.UserId,
                    CryptocurrencyId = trx.CryptocurrencyId,
                    CryptocurrencySymbol = trx.Cryptocurrency.Symbol,
                    Type = trx.Type,
                    Amount = trx.Amount,
                    Price = trx.Price,
                    TotalValue = trx.TotalValue,
                    Timestamp = trx.Timestamp
                })
                .ToListAsync();
        }

        public async Task<TransactionDto?> GetTransactionDetailsAsync(int transactionId) 
        {
            var trx = await _context.Transactions
                .Include(t => t.Cryptocurrency)
                .Include(t => t.User) 
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (trx == null) return null;

            return new TransactionDto
            {
                Id = trx.Id,
                UserId = trx.UserId,
                CryptocurrencyId = trx.CryptocurrencyId,
                CryptocurrencySymbol = trx.Cryptocurrency.Symbol,
                Type = trx.Type,
                Amount = trx.Amount,
                Price = trx.Price,
                TotalValue = trx.TotalValue,
                Timestamp = trx.Timestamp
            };
        }

        public async Task<TotalProfitDto> CalculateUserProfitAsync(int userId) 
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

            return new TotalProfitDto { TotalProfit = totalProfit };
        }

        public async Task<DetailedProfitDto> GetDetailedProfitAsync(int userId) 
        {
            var walletCryptos = await _context.WalletCryptos
                .Include(wc => wc.Cryptocurrency)
                .Where(wc => wc.Wallet.UserId == userId)
                .ToListAsync();

            var result = new List<CryptoProfitDetailDto>();
            decimal totalProfit = 0;

            foreach (var walletCrypto in walletCryptos)
            {
                decimal currentValue = walletCrypto.Amount * walletCrypto.Cryptocurrency.CurrentPrice;
                decimal acquisitionValue = walletCrypto.Amount * walletCrypto.AverageAcquisitionPrice;
                decimal profit = currentValue - acquisitionValue;
                totalProfit += profit;

                result.Add(new CryptoProfitDetailDto 
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

            return new DetailedProfitDto 
            {
                DetailedProfits = result,
                TotalProfit = totalProfit
            };
        }
    }
}