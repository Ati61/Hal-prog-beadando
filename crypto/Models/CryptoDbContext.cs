// Data/CryptoDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace crypto.Models
{
    public class CryptoDbContext : DbContext
    {
        public CryptoDbContext(DbContextOptions<CryptoDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Cryptocurrency> Cryptocurrencies { get; set; }
        public DbSet<WalletCrypto> WalletCryptos { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PriceHistory> PriceHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User to Wallet relationship (one-to-one)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId);

            // User to Transaction relationship (one-to-many) 
            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            // Wallet to WalletCrypto relationship (one-to-many)
            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.WalletCryptos)
                .WithOne(wc => wc.Wallet)
                .HasForeignKey(wc => wc.WalletId);

            // Cryptocurrency to WalletCrypto relationship (one-to-many)
            modelBuilder.Entity<Cryptocurrency>()
                .HasMany(c => c.WalletCryptos)
                .WithOne(wc => wc.Cryptocurrency)
                .HasForeignKey(wc => wc.CryptocurrencyId);

            // Cryptocurrency to PriceHistory relationship (one-to-many)
            modelBuilder.Entity<Cryptocurrency>()
                .HasMany(c => c.PriceHistory)
                .WithOne(ph => ph.Cryptocurrency)
                .HasForeignKey(ph => ph.CryptocurrencyId);

            // Performance indexes 
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.UserId);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.Timestamp);

            modelBuilder.Entity<WalletCrypto>()
                .HasIndex(wc => new { wc.WalletId, wc.CryptocurrencyId })
                .IsUnique();

            // Cryptocurrency decimal properties
            modelBuilder.Entity<Cryptocurrency>()
                .Property(c => c.CurrentPrice)
                .HasPrecision(18, 8);

            // PriceHistory decimal properties
            modelBuilder.Entity<PriceHistory>()
                .Property(ph => ph.Price)
                .HasPrecision(18, 8);

            // Transaction decimal properties
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Price)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.TotalValue)
                .HasPrecision(18, 8);

            // Wallet decimal properties
            modelBuilder.Entity<Wallet>()
                .Property(w => w.Balance)
                .HasPrecision(18, 8);

            // WalletCrypto decimal properties
            modelBuilder.Entity<WalletCrypto>()
                .Property(wc => wc.Amount)
                .HasPrecision(18, 8);

            modelBuilder.Entity<WalletCrypto>()
                .Property(wc => wc.AverageAcquisitionPrice)
                .HasPrecision(18, 8);
        }
    }
}