using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Interfaces.Hubs
{
    public interface IAdminHubService
    {
        Task AdminNewShipmentMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminShipmentUpdatedMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminNewApiClientMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminApiClientUpdatedMessageAsync(string message, CancellationToken cancellationToken = default);
        Task AdminShipmentIssueMessageAsync(string message, CancellationToken cancellationToken = default);
    }
}
