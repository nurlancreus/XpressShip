using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Abstractions.Hubs
{
    public interface IAdminHubService
    {
        Task AdminNewShipmentMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminShipmentDeliveredMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminShipmentUpdatedMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminShipmentIssueMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminNewApiClientMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminApiClientUpdatedMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminNewAdminMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminNewSenderMessageAsync(string message, CancellationToken cancellationToken = default);
    }
}
