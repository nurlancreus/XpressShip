using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Interfaces.Services.Payment
{
    public interface IPaymentGatewayService
    {
        Task<string> CreatePaymentAsync(Shipment shipment, CancellationToken cancellationToken = default);
        Task<bool> CapturePaymentAsync(string transactionId, CancellationToken cancellationToken = default);
        Task<bool> CancelPaymentAsync(string transactionId, CancellationToken cancellationToken = default);
        Task<bool> RefundPaymentAsync(string transactionId, CancellationToken cancellationToken = default);
    }
}
