using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XpressShip.Application.Abstractions.Hubs;
using XpressShip.Application.Abstractions.Services.Notification;
using XpressShip.Domain.Enums;
using XpressShip.Infrastructure.Persistence;

namespace XpressShip.Infrastructure.Services.Background
{
    public class ShipmentDeliveryNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ShipmentDeliveryNotificationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public ShipmentDeliveryNotificationService(IServiceProvider serviceProvider, ILogger<ShipmentDeliveryNotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_checkInterval);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var adminNotificationService = scope.ServiceProvider.GetRequiredService<IAdminNotificationService>();

                _logger.LogInformation("Checking for overdue shipments...");

                var overdueShipments = dbContext.Shipments
                    .Where(s => s.EstimatedDate <= DateTime.UtcNow && s.Status != ShipmentStatus.Delivered)
                    .ToList();

                if (overdueShipments.Count != 0)
                {
                    foreach (var shipment in overdueShipments)
                    {
                        await adminNotificationService.SendShipmentDeliveredNotificationAsync(shipment, stoppingToken);

                        _logger.LogInformation($"Admin notified for overdue shipment: {shipment.TrackingNumber}");
                    }
                }
            }
        }
    }
}
