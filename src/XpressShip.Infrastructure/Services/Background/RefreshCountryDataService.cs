using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XpressShip.Application.Options.Cache;
using XpressShip.Domain.Validation;

namespace XpressShip.Infrastructure.Services.Background
{
    public class RefreshCountryDataService : BackgroundService
    {
        private readonly TimeSpan _checkInterval;
        private readonly ILogger<RefreshCountryDataService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RefreshCountryDataService(IServiceProvider serviceProvider,
            IOptionsSnapshot<InMemoryCacheSettings> options, ILogger<RefreshCountryDataService> logger)
        {
            var absoluteExpirationInHours = TimeSpan.FromHours(options.Get(InMemoryCacheSettings.CountryData).AbsoluteExpirationInHours);

            _checkInterval = absoluteExpirationInHours + TimeSpan.FromSeconds(1);

            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_checkInterval);

            while (!stoppingToken.IsCancellationRequested &&
                       await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _serviceProvider.CreateScope();
                var addressValidationService = scope.ServiceProvider.GetRequiredService<IAddressValidationService>();

                try
                {
                    _logger.LogInformation("Refreshing country data cache...");
                    await addressValidationService.RefreshCountriesCacheAsync(stoppingToken);
                    _logger.LogInformation("Country data cache refreshed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing country data cache.");
                }
            }
        }
    }
}
