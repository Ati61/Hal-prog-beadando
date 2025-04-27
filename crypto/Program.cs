using crypto.Interfaces;
using crypto.Models;
using crypto.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



// Szolgáltatások hozzáadása a konténerhez.

// Adatbázis kontextus
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")?.Replace("Y4M52R", "Y4M52R"); // Cseréld le a saját Neptun kódodra, ha szükséges
builder.Services.AddDbContext<CryptoDbContext>(options =>
    options.UseSqlServer(connectionString));

// Alkalmazás szolgáltatások regisztrálása
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ICryptocurrencyService, CryptocurrencyService>();
builder.Services.AddScoped<ITradeService, TradeService>();

// Háttérszolgáltatás regisztrálása az árfolyamfrissítésekhez
builder.Services.AddHostedService<PriceUpdateService>();

builder.Services.AddControllers();
// További információ a Swagger/OpenAPI konfigurálásáról: https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Adatbázis feltöltése indításkor
await SeedDatabaseAsync(app.Services);

// HTTP kérés pipeline konfigurálása.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


// Segédmetódus az adatbázis feltöltéséhez
async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<CryptoDbContext>();
            var cryptoService = services.GetRequiredService<ICryptocurrencyService>();
            var userService = services.GetRequiredService<IUserService>(); // UserService lekérése
            var logger = services.GetRequiredService<ILogger<Program>>(); // Logger lekérése

            logger.LogInformation("Adatbázis migrációk alkalmazása...");
            // Függőben lévő migrációk alkalmazása
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations applied.");

            // Kezdeti kriptovaluták feltöltése, ha még nincsenek
            if (!await context.Cryptocurrencies.AnyAsync())
            {
                logger.LogInformation("Seeding initial cryptocurrencies...");
                await cryptoService.SeedInitialCryptocurrenciesAsync();
                logger.LogInformation("Kezdeti kriptovaluták feltöltve.");
            }

            // Kezdeti felhasználók feltöltése, ha még nincsenek (Példa)
            User? user1 = null; // Változók deklarálása az if-en kívül
            User? user2 = null;
            if (!await context.Users.AnyAsync())
            {
                logger.LogInformation("Seeding initial users...");
                var user1Dto = await userService.RegisterUserAsync(new crypto.Dtos.UserRegisterDto { Username = "TestUser1", Email = "test1@example.com", Password = "password123" });
                var user2Dto = await userService.RegisterUserAsync(new crypto.Dtos.UserRegisterDto { Username = "TestUser2", Email = "test2@example.com", Password = "password456" });
                logger.LogInformation("Kezdeti felhasználók feltöltve.");
                // Teljes felhasználói entitások lekérése Wallet-tel együtt a birtoklások feltöltéséhez
                // Használj FirstOrDefaultAsync-t a biztonság kedvéért, bár a regisztráció után létezniük kell
                user1 = await context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == user1Dto.Id);
                user2 = await context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == user2Dto.Id);
            }

             // Kezdeti tárca birtoklások feltöltése teszt felhasználókhoz, ha a WalletCryptos üres
            if (!await context.WalletCryptos.AnyAsync())
            {
                 logger.LogInformation("Seeding initial wallet holdings...");
                 // Biztosítsd, hogy a felhasználók be lettek töltve, vagy kérd le őket, ha már léteztek
                 if (user1 == null) user1 = await context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Email == "test1@example.com");
                 if (user2 == null) user2 = await context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Email == "test2@example.com");

                // Szükséges kriptovaluták lekérése
                var btc = await context.Cryptocurrencies.FirstOrDefaultAsync(c => c.Symbol == "BTC");
                var eth = await context.Cryptocurrencies.FirstOrDefaultAsync(c => c.Symbol == "ETH");
                var doge = await context.Cryptocurrencies.FirstOrDefaultAsync(c => c.Symbol == "DOGE");

                bool addedHoldings = false; // Zászló annak ellenőrzésére, hogy hozzáadtunk-e valamit

                // Feltöltés User1 számára
                if (user1?.Wallet != null && btc != null)
                {
                    context.WalletCryptos.Add(new WalletCrypto
                    {
                        WalletId = user1.Wallet.Id,
                        CryptocurrencyId = btc.Id,
                        Amount = 0.1m,
                        AverageAcquisitionPrice = 58000m // Példa beszerzési ár
                    });
                    logger.LogInformation("Added BTC holding for User1.");
                    addedHoldings = true;
                }
                 if (user1?.Wallet != null && eth != null)
                {
                     context.WalletCryptos.Add(new WalletCrypto
                    {
                        WalletId = user1.Wallet.Id,
                        CryptocurrencyId = eth.Id,
                        Amount = 2.5m,
                        AverageAcquisitionPrice = 2900m // Példa beszerzési ár
                    });
                     logger.LogInformation("Added ETH holding for User1.");
                    addedHoldings = true;
                }

                // Feltöltés User2 számára
                 if (user2?.Wallet != null && doge != null)
                {
                     context.WalletCryptos.Add(new WalletCrypto
                    {
                        WalletId = user2.Wallet.Id,
                        CryptocurrencyId = doge.Id,
                        Amount = 10000m,
                        AverageAcquisitionPrice = 0.12m // Példa beszerzési ár
                    });
                     logger.LogInformation("Added DOGE holding for User2.");
                    addedHoldings = true;
                }

                // Változások mentése csak akkor, ha birtoklásokat adtunk hozzá
                if(addedHoldings)
                {
                    await context.SaveChangesAsync();
                    logger.LogInformation("Initial wallet holdings saved.");
                }
                else
                {
                    logger.LogInformation("No initial wallet holdings needed or users/cryptos not found.");
                }
            }


            logger.LogInformation("Adatbázis feltöltés ellenőrzése befejeződött.");
        }
        catch (Exception ex)
        {
            // Használd a szolgáltatótól kapott loggert
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }
}
