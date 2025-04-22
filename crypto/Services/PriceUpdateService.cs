using crypto.Interfaces;
using crypto.Services;

namespace crypto.Services
{
    public class PriceUpdateService : BackgroundService
    {
        private readonly ILogger<PriceUpdateService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(60); // 60 másodpercenként frissít

        public PriceUpdateService(
            ILogger<PriceUpdateService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Price Update Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Price Update Service is updating prices at: {time}", DateTimeOffset.Now);

                try
                {
                    await UpdatePricesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating prices.");
                }

                await Task.Delay(_updateInterval, stoppingToken);
            }
        }

        private async Task UpdatePricesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var cryptocurrencyService = scope.ServiceProvider.GetRequiredService<ICryptocurrencyService>();
            
            if (cryptocurrencyService is CryptocurrencyService service)
            {
                await service.UpdateAllPricesRandomlyAsync();
            }
        }
    }
}