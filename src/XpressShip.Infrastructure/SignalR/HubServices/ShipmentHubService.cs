using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Hubs;
using XpressShip.Domain.Enums;
using XpressShip.Infrastructure.SignalR.Constants;
using XpressShip.Infrastructure.SignalR.Hubs;

namespace XpressShip.Infrastructure.SignalR.HubServices
{
    public class ShipmentHubService : IShipmentHubService
    {
        private readonly IHubContext<ShipmentHub> _hubContext;

        public ShipmentHubService(IHubContext<ShipmentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task ShipmentShippedMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Groups([GroupNames.AdminGroup, Helper.GetUserGroupName(identifier, userType)]).SendAsync(ReceiveFunctionNames.ShipmentHub.ShipmentShippedMessage, message, cancellationToken);
        }

        public async Task ShipmentCanceledMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Groups([GroupNames.AdminGroup, Helper.GetUserGroupName(identifier, userType)]).SendAsync(ReceiveFunctionNames.ShipmentHub.ShipmentCanceledMessage, message, cancellationToken);
        }

        public async Task ShipmentDelayedMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Groups([GroupNames.AdminGroup, Helper.GetUserGroupName(identifier, userType)]).SendAsync(ReceiveFunctionNames.ShipmentHub.ShipmentDelayedMessage, message, cancellationToken);
        }

        public async Task ShipmentDeliveredMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Groups([GroupNames.AdminGroup, Helper.GetUserGroupName(identifier, userType)]).SendAsync(ReceiveFunctionNames.ShipmentHub.ShipmentDeliveredMessage, message, cancellationToken);
        }

        public async Task ShipmentFailedMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Groups([GroupNames.AdminGroup, Helper.GetUserGroupName(identifier, userType)]).SendAsync(ReceiveFunctionNames.ShipmentHub.ShipmentFailedMessage, message, cancellationToken);
        }
    }
}
