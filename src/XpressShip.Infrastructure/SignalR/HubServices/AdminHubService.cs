using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Hubs;
using XpressShip.Infrastructure.SignalR.Constants;
using XpressShip.Infrastructure.SignalR.Hubs;

namespace XpressShip.Infrastructure.SignalR.HubServices
{
    public class AdminHubService : IAdminHubService
    {
        private readonly IHubContext<AdminHub> _hubContext;

        public AdminHubService(IHubContext<AdminHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task AdminApiClientUpdatedMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients.Group(GroupNames.AdminGroup).SendAsync(ReceiveFunctionNames.AdminHub.AdminNewApiClientMessage, message, cancellationToken);
        }

        public Task AdminNewAdminMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients.Group(GroupNames.AdminGroup).SendAsync(ReceiveFunctionNames.AdminHub.AdminNewAdminMessage, message, cancellationToken);
        }

        public Task AdminNewSenderMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients.Group(GroupNames.AdminGroup).SendAsync(ReceiveFunctionNames.AdminHub.AdminNewSenderMessage, message, cancellationToken);
        }

        public Task AdminNewApiClientMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients.Group(GroupNames.AdminGroup).SendAsync(ReceiveFunctionNames.AdminHub.AdminNewApiClientMessage, message, cancellationToken);
        }

        public Task AdminNewShipmentMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients.Group(GroupNames.AdminGroup).SendAsync(ReceiveFunctionNames.AdminHub.AdminNewShipmentMessage, message, cancellationToken);
        }

        public Task AdminShipmentIssueMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients.Group(GroupNames.AdminGroup).SendAsync(ReceiveFunctionNames.AdminHub.AdminShipmentIssueMessage, message, cancellationToken);
        }

        public Task AdminShipmentUpdatedMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients.Group(GroupNames.AdminGroup).SendAsync(ReceiveFunctionNames.AdminHub.AdminShipmentUpdatedMessage, message, cancellationToken);
        }

        public Task AdminShipmentDeliveredMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients.Group(GroupNames.AdminGroup).SendAsync(ReceiveFunctionNames.AdminHub.AdminShipmentDeliveredMessage, message, cancellationToken);
        }
    }
}
