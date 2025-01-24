using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Services.Payment;
using XpressShip.Application.Abstractions.Services.Payment.Stripe;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using PaymentEntity = XpressShip.Domain.Entities.Payment;

namespace XpressShip.Infrastructure.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IStripeService _stripeService;

        public PaymentService(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }

        public Task<string> CreatePaymentAsync(Shipment shipment, CancellationToken cancellationToken = default)
        {
            return shipment.Payment!.Method switch
            {
                PaymentMethod.DebitCard => _stripeService.CreatePaymentAsync(shipment, cancellationToken),
                _ => throw new ArgumentException("Payment method is not specified")
            };
        }

        public Task<bool> CancelPaymentAsync(PaymentEntity payment, CancellationToken cancellationToken = default)
        {
            return payment.Method switch
            {
                PaymentMethod.DebitCard => _stripeService.CancelPaymentAsync(payment.TransactionId, cancellationToken),
                _ => throw new ArgumentException("Payment method is not specified")
            };
        }

        public Task<bool> CapturePaymentAsync(PaymentEntity payment, CancellationToken cancellationToken = default)
        {
            return payment.Method switch
            {
                PaymentMethod.DebitCard => _stripeService.CapturePaymentAsync(payment.TransactionId, cancellationToken),
                _ => throw new ArgumentException("Payment method is not specified")
            };
        }

        public Task<bool> RefundPaymentAsync(PaymentEntity payment, CancellationToken cancellationToken = default)
        {
            return payment.Method switch
            {
                PaymentMethod.DebitCard => _stripeService.RefundPaymentAsync(payment.TransactionId, cancellationToken),
                _ => throw new ArgumentException("Payment method is not specified")
            };
        }
    }
}
