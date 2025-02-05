using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Abstractions.Services.Notification
{
    public interface IAdminNotificationService
    {
        // Shipment Notifications
        Task SendShipmentDeliveredNotificationAsync(Shipment shipment, CancellationToken cancellationToken = default);
        Task SendNewShipmentNotificationAsync(Shipment shipment, CancellationToken cancellationToken = default);
        Task SendShipmentUpdatedNotificationAsync(Shipment shipment, CancellationToken cancellationToken = default);
        Task SendShipmentIssueNotificationAsync(Shipment shipment, string issueMessage, CancellationToken cancellationToken = default);

        // ApiClient Notifications
        Task SendNewApiClientNotificationAsync(ApiClient apiClient, CancellationToken cancellationToken = default);
        Task SendApiClientUpdatedNotificationAsync(ApiClient apiClient, CancellationToken cancellationToken = default);

        // User Notifications
        Task SendNewAdminNotificationAsync(Admin admin, CancellationToken cancellationToken = default);
        Task SendNewSenderNotificationAsync(Sender sender, CancellationToken cancellationToken = default);
    }
}
