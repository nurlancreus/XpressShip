using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities;
using PaymentEntity = XpressShip.Domain.Entities.Payment;

namespace XpressShip.Application.Interfaces.Services.Payment
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentAsync(Shipment shipment, CancellationToken cancellationToken = default);
        Task<bool> CapturePaymentAsync(PaymentEntity payment, CancellationToken cancellationToken = default);
        Task<bool> CancelPaymentAsync(PaymentEntity payment, CancellationToken cancellationToken = default);
        Task<bool> RefundPaymentAsync(PaymentEntity payment, CancellationToken cancellationToken = default);
    }
}
