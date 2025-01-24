using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Hubs;
using XpressShip.Domain.Enums;
using XpressShip.Infrastructure.SignalR.Constants;
using XpressShip.Infrastructure.SignalR.Hubs;

namespace XpressShip.Infrastructure.SignalR.HubServices
{
    public class PaymentHubService : IPaymentHubService
    {
        private readonly IHubContext<PaymentHub> _hubContext;

        public PaymentHubService(IHubContext<PaymentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PaymentCanceledMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default)
        {
            
            await _hubContext.Clients.Groups([GroupNames.AdminGroup, Helper.GetUserGroupName(identifier, userType)]).SendAsync(ReceiveFunctionNames.PaymentHub.PaymentCanceledMessage, message, cancellationToken);
        }

        public async Task PaymentFailedMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Groups([GroupNames.AdminGroup, Helper.GetUserGroupName(identifier, userType)]).SendAsync(ReceiveFunctionNames.PaymentHub.PaymentFailedMessage, message, cancellationToken);
        }

        public async Task PaymentRefundedMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Groups([GroupNames.AdminGroup, Helper.GetUserGroupName(identifier, userType)]).SendAsync(ReceiveFunctionNames.PaymentHub.PaymentRefundedMessage, message, cancellationToken);
        }

        public async Task PaymentSucceededMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Groups([GroupNames.AdminGroup, Helper.GetUserGroupName(identifier, userType)]).SendAsync(ReceiveFunctionNames.PaymentHub.PaymentSucceededMessage, message, cancellationToken);
        }
    }
}
