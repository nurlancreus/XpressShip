using XpressShip.Application.Abstractions.Hubs;
using XpressShip.Application.Abstractions.Services.Notification;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Infrastructure.Services.Notification
{
    public class AdminNotificationService : IAdminNotificationService
    {
        private readonly IAdminHubService _adminHubService;

        public AdminNotificationService(IAdminHubService adminHubService)
        {
            _adminHubService = adminHubService;
        }

        // Shipment Notifications
        public async Task SendShipmentDeliveredNotificationAsync(Shipment shipment, CancellationToken cancellationToken = default)
        {
            await _adminHubService.AdminShipmentDeliveredMessageAsync($"Shipment with tracking number {shipment.TrackingNumber} was expected to be delivered on {shipment.EstimatedDate}. Please update the status.", cancellationToken);
        }

        public async Task SendNewShipmentNotificationAsync(Shipment shipment, CancellationToken cancellationToken = default)
        {
            await _adminHubService.AdminNewShipmentMessageAsync($"A new shipment has been created with tracking number {shipment.TrackingNumber}.", cancellationToken);
        }

        public async Task SendShipmentUpdatedNotificationAsync(Shipment shipment, CancellationToken cancellationToken = default)
        {
            await _adminHubService.AdminShipmentUpdatedMessageAsync($"Shipment with tracking number {shipment.TrackingNumber} has been updated. Current status: {shipment.Status}.", cancellationToken);
        }

        public async Task SendShipmentIssueNotificationAsync(Shipment shipment, string issueMessage, CancellationToken cancellationToken = default)
        {
            await _adminHubService.AdminShipmentIssueMessageAsync($"Issue reported for shipment {shipment.TrackingNumber}: {issueMessage}", cancellationToken);
        }

        // ApiClient Notifications
        public async Task SendNewApiClientNotificationAsync(ApiClient apiClient, CancellationToken cancellationToken = default)
        {
            await _adminHubService.AdminNewApiClientMessageAsync($"A new API client has been registered: {apiClient.CompanyName}.", cancellationToken);
        }

        public async Task SendApiClientUpdatedNotificationAsync(ApiClient apiClient, CancellationToken cancellationToken = default)
        {
            await _adminHubService.AdminApiClientUpdatedMessageAsync($"API client {apiClient.CompanyName} has been updated.", cancellationToken);
        }

        // User Notifications
        public async Task SendNewAdminNotificationAsync(Admin admin, CancellationToken cancellationToken = default)
        {
            await _adminHubService.AdminNewAdminMessageAsync($"A new admin has been registered: {admin.UserName}. Activate it.", cancellationToken);
        }
        public async Task SendNewSenderNotificationAsync(Sender sender, CancellationToken cancellationToken = default)
        {
            await _adminHubService.AdminNewSenderMessageAsync($"A new sender has been registered: {sender.UserName}. Activate it.", cancellationToken);
        }
    }

}
