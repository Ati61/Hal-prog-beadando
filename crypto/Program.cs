using crypto.Models;
using Microsoft.EntityFrameworkCore;
using crypto.Services;
using crypto.Interfaces;

namespace crypto
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Connection string beolvasása a konfigurációból
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<CryptoDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Regisztráljuk a szolgáltatásokat
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IWalletService, WalletService>();
            builder.Services.AddScoped<ICryptocurrencyService, CryptocurrencyService>();
            builder.Services.AddScoped<ITradeService, TradeService>();

            // Regisztráljuk az árfolyam frissítő háttérszolgáltatást
            builder.Services.AddHostedService<PriceUpdateService>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Adatbázis inicializálása
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var cryptoService = services.GetRequiredService<ICryptocurrencyService>();
                    // Kezdő kriptovaluták feltöltése az adatbázisba
                    cryptoService.SeedInitialCryptocurrenciesAsync().Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Hiba történt az adatbázis inicializálásakor.");
                }
            }

            app.Run();
        }
    }
}
