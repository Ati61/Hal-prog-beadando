using crypto.Interfaces;
using crypto.Models;
using crypto.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; 
using System.Linq; 

namespace crypto.Services
{
    public class CryptocurrencyService : ICryptocurrencyService
    {
        private readonly CryptoDbContext _context;
        private readonly Random _random = new Random();

        public CryptocurrencyService(CryptoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CryptocurrencyDto>> GetAllCryptocurrenciesAsync() 
        {
            return await _context.Cryptocurrencies
                .Select(c => new CryptocurrencyDto { Id = c.Id, Symbol = c.Symbol, Name = c.Name, CurrentPrice = c.CurrentPrice })
                .ToListAsync();
        }

        public async Task<CryptocurrencyDto?> GetCryptocurrencyByIdAsync(int id) 
        {
            var crypto = await _context.Cryptocurrencies.FindAsync(id);
            if (crypto == null) return null;
            return new CryptocurrencyDto { Id = crypto.Id, Symbol = crypto.Symbol, Name = crypto.Name, CurrentPrice = crypto.CurrentPrice };
        }

        public async Task<CryptocurrencyDto> AddCryptocurrencyAsync(CryptocurrencyCreateDto cryptoDto) 
        {
            if (await _context.Cryptocurrencies.AnyAsync(c => c.Symbol == cryptoDto.Symbol))
            {
                throw new InvalidOperationException("Cryptocurrency with this symbol already exists.");
            }

            var cryptocurrency = new Cryptocurrency
            {
                Symbol = cryptoDto.Symbol,
                Name = cryptoDto.Name,
                CurrentPrice = cryptoDto.CurrentPrice
            };

            _context.Cryptocurrencies.Add(cryptocurrency);
            await _context.SaveChangesAsync(); 

            // Kezdeti ár rögzítése az előzményekbe
            var priceHistory = new PriceHistory
            {
                CryptocurrencyId = cryptocurrency.Id,
                Price = cryptocurrency.CurrentPrice,
                Timestamp = DateTime.UtcNow
            };
            _context.PriceHistory.Add(priceHistory);
            await _context.SaveChangesAsync();

            return new CryptocurrencyDto { Id = cryptocurrency.Id, Symbol = cryptocurrency.Symbol, Name = cryptocurrency.Name, CurrentPrice = cryptocurrency.CurrentPrice };
        }

        public async Task<bool> DeleteCryptocurrencyAsync(int id)
        {
            var crypto = await _context.Cryptocurrencies.FindAsync(id);
            if (crypto == null)
            {
                return false;
            }

            _context.Cryptocurrencies.Remove(crypto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CryptocurrencyDto?> UpdateCryptoPriceAsync(int id, decimal newPrice)
        {
            var crypto = await _context.Cryptocurrencies.FindAsync(id);
            if (crypto == null)
            {
                return null;
            }

            crypto.CurrentPrice = newPrice;

            var priceHistory = new PriceHistory
            {
                CryptocurrencyId = id,
                Price = newPrice,
                Timestamp = DateTime.UtcNow
            };
            _context.PriceHistory.Add(priceHistory);

            await _context.SaveChangesAsync();
            return new CryptocurrencyDto { Id = crypto.Id, Symbol = crypto.Symbol, Name = crypto.Name, CurrentPrice = crypto.CurrentPrice };
        }

        public async Task<IEnumerable<PriceHistoryDto>> GetPriceHistoryAsync(int cryptoId) // Return DTO
        {
            return await _context.PriceHistory
                .Where(ph => ph.CryptocurrencyId == cryptoId)
                .OrderByDescending(ph => ph.Timestamp)
                .Select(ph => new PriceHistoryDto { Id = ph.Id, CryptocurrencyId = ph.CryptocurrencyId, Price = ph.Price, Timestamp = ph.Timestamp })
                .ToListAsync();
        }

        public async Task SeedInitialCryptocurrenciesAsync()
        {
            if (await _context.Cryptocurrencies.AnyAsync())
            {
                return;
            }

            var cryptocurrencies = new List<Cryptocurrency>
            {
                new Cryptocurrency { Symbol = "BTC", Name = "Bitcoin", CurrentPrice = 60000.00m },
                new Cryptocurrency { Symbol = "ETH", Name = "Ethereum", CurrentPrice = 3000.00m },
                new Cryptocurrency { Symbol = "BNB", Name = "Binance Coin", CurrentPrice = 450.00m },
                new Cryptocurrency { Symbol = "SOL", Name = "Solana", CurrentPrice = 120.00m },
                new Cryptocurrency { Symbol = "ADA", Name = "Cardano", CurrentPrice = 0.50m },
                new Cryptocurrency { Symbol = "XRP", Name = "Ripple", CurrentPrice = 0.60m },
                new Cryptocurrency { Symbol = "DOT", Name = "Polkadot", CurrentPrice = 15.00m },
                new Cryptocurrency { Symbol = "DOGE", Name = "Dogecoin", CurrentPrice = 0.15m },
                new Cryptocurrency { Symbol = "AVAX", Name = "Avalanche", CurrentPrice = 35.00m },
                new Cryptocurrency { Symbol = "MATIC", Name = "Polygon", CurrentPrice = 1.20m },
                new Cryptocurrency { Symbol = "LINK", Name = "Chainlink", CurrentPrice = 18.00m },
                new Cryptocurrency { Symbol = "UNI", Name = "Uniswap", CurrentPrice = 10.00m },
                new Cryptocurrency { Symbol = "ALGO", Name = "Algorand", CurrentPrice = 0.30m },
                new Cryptocurrency { Symbol = "XLM", Name = "Stellar", CurrentPrice = 0.12m },
                new Cryptocurrency { Symbol = "ATOM", Name = "Cosmos", CurrentPrice = 12.00m }
            };

            foreach (var crypto in cryptocurrencies)
            {
                _context.Cryptocurrencies.Add(crypto);
            }
            await _context.SaveChangesAsync();

            // Kezdeti árfolyam-előzmények hozzáadása minden kriptovalutához
            var now = DateTime.UtcNow;
            foreach (var crypto in cryptocurrencies)
            {
                _context.PriceHistory.Add(new PriceHistory
                {
                    CryptocurrencyId = crypto.Id,
                    Price = crypto.CurrentPrice,
                    Timestamp = now
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAllPricesRandomlyAsync()
        {
            var cryptos = await _context.Cryptocurrencies.ToListAsync();
            var now = DateTime.UtcNow;

            foreach (var crypto in cryptos)
            {
                // Véletlenszerű százalékos változás generálása -5% és +5% között
                var percentageChange = (_random.NextDouble() * 10) - 5;
                var priceChange = crypto.CurrentPrice * (decimal)(percentageChange / 100);
                var newPrice = Math.Max(0.00000001m, crypto.CurrentPrice + priceChange);
                
                crypto.CurrentPrice = newPrice;

                // Hozzáadás az árfolyam-előzményekhez
                _context.PriceHistory.Add(new PriceHistory
                {
                    CryptocurrencyId = crypto.Id,
                    Price = newPrice,
                    Timestamp = now
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}