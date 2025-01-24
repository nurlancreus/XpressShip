using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Abstractions.Hubs
{
    public interface IShipmentHubService
    {
        Task ShipmentShippedMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default);
        Task ShipmentDeliveredMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default);
        Task ShipmentDelayedMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default);
        Task ShipmentCanceledMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default);
        Task ShipmentFailedMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default);
    }
}
