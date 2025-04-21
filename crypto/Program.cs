using crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace crypto
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            string connectionString = "Server=(local);Database=CryptoDb_Y4M52R;Trusted_Connection=True;TrustServerCertificate=True;";
            builder.Services.AddDbContext<CryptoDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //builder.Services.AddHostedService<PriceUpdateService>();

            //builder.Services.AddScoped<IUserService, UserService>();
            //builder.Services.AddScoped<IWalletService, WalletService>();
            //builder.Services.AddScoped<ICryptocurrencyService, CryptocurrencyService>();
            //builder.Services.AddScoped<ITradeService, TradeService>();

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

            app.Run();
        }
    }
}
