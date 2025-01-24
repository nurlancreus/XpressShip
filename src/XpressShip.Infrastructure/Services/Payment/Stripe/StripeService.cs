using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using XpressShip.Application.Abstractions.Services.Payment.Stripe;
using XpressShip.Application.Options.PaymentGateway;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Services.Payment.Stripe
{
    public class StripeService : IStripeService
    {

        public StripeService(IOptions<PaymentGatewaySettings> options)
        {

            StripeConfiguration.ApiKey = options.Value.Stripe.SecretKey;
        }

        public async Task<string> CreatePaymentAsync(Shipment shipment, CancellationToken cancellationToken = default)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(shipment.Cost * 100), // cents
                Currency = shipment.Payment!.Currency.ToString().ToLower(),
                PaymentMethodTypes = ["card"],
                Shipping = new ChargeShippingOptions
                {
                    Address = new AddressOptions
                    {
                        City = shipment.DestinationAddress.City.Name,
                        Country = shipment.DestinationAddress.City.Country.Name,
                        PostalCode = shipment.DestinationAddress.PostalCode,
                        Line1 = shipment.DestinationAddress.Street,
                    },
                    Name = shipment.ApiClient is not null ? shipment.ApiClient.CompanyName : "SenderName",
                    Carrier = "XpressShip",
                    // Phone = "+9949874564",
                    TrackingNumber = shipment.TrackingNumber,
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);

            return paymentIntent.Id;
        }

        public async Task<bool> CapturePaymentAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.CaptureAsync(transactionId, cancellationToken: cancellationToken);

            return paymentIntent.Status == "succeeded";
        }

        public async Task<bool> RefundPaymentAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = transactionId,
            };

            var service = new RefundService();
            var refund = await service.CreateAsync(options, cancellationToken: cancellationToken);

            return refund.Status == "succeeded";
        }

        public async Task<bool> CancelPaymentAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.CancelAsync(transactionId, cancellationToken: cancellationToken);

            return paymentIntent.Status == "canceled";
        }
    }
}
