using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Abstractions.Hubs
{
    public interface IPaymentHubService
    {
        Task PaymentSucceededMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default);
        Task PaymentRefundedMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default);
        Task PaymentCanceledMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default);
        Task PaymentFailedMessageAsync(string identifier, string message, UserType userType = UserType.ApiClient, CancellationToken cancellationToken = default);
    }
}
