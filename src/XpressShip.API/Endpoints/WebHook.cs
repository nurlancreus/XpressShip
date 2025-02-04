using MediatR;
using Microsoft.Extensions.Options;
using Stripe;
using XpressShip.Application.Notifications.Payment;
using XpressShip.Application.Options.PaymentGateway;

namespace XpressShip.API.Endpoints
{
    public static class WebHook
    {
        public static IEndpointRouteBuilder RegisterWebHookEndpoints(this IEndpointRouteBuilder routes)
        {
            var webHooks = routes.MapGroup("/api/webhook").AllowAnonymous();

            webHooks.MapPost("stripe", async (HttpRequest request, IMediator mediator, IOptions<PaymentGatewaySettings> options, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            {
                var logger = loggerFactory.CreateLogger(typeof(WebHook));

                var webHookSecret = options.Value.Stripe.WebhookSecret;

                var json = await new StreamReader(request.Body).ReadToEndAsync(cancellationToken);

                try
                {
                    var stripeSignature = request.Headers["Stripe-Signature"];
                    Event stripeEvent = EventUtility.ConstructEvent(
                        json,
                        stripeSignature,
                        webHookSecret
                    );

                    var transactionId = string.Empty;

                    switch (stripeEvent.Type)
                    {
                        case EventTypes.PaymentIntentSucceeded:
                            transactionId = ((PaymentIntent)stripeEvent.Data.Object).Id;
                            logger.LogInformation("PaymentIntent succeeded. Transaction ID: {TransactionId}", transactionId);
                            await mediator.Publish(new PaymentSucceededNotification { TransactionId = transactionId }, cancellationToken);
                            break;

                        case EventTypes.PaymentIntentCanceled:
                            transactionId = ((PaymentIntent)stripeEvent.Data.Object).Id;
                            logger.LogInformation("PaymentIntent canceled. Transaction ID: {TransactionId}", transactionId);
                            await mediator.Publish(new PaymentCanceledNotification { TransactionId = transactionId }, cancellationToken);
                            break;

                        case EventTypes.PaymentIntentPaymentFailed:
                            transactionId = ((PaymentIntent)stripeEvent.Data.Object).Id;
                            logger.LogWarning("PaymentIntent failed. Transaction ID: {TransactionId}", transactionId);
                            await mediator.Publish(new PaymentFailedNotification { TransactionId = transactionId }, cancellationToken);
                            break;

                        case EventTypes.ChargeRefunded:
                            transactionId = ((Charge)stripeEvent.Data.Object).PaymentIntentId;
                            logger.LogInformation("Charge refunded. Transaction ID: {TransactionId}", transactionId);
                            await mediator.Publish(new PaymentRefundedNotification { TransactionId = transactionId }, cancellationToken);
                            break;

                        default:
                            logger.LogWarning("Unhandled event type: {EventType}", stripeEvent.Type);
                            break;
                    }

                    logger.LogInformation("Stripe webhook event processed successfully. Event Type: {EventType}", stripeEvent.Type);
                    return Results.Ok();
                }
                catch (StripeException ex)
                {
                    logger.LogError(ex, "StripeException occurred while processing webhook.");
                    return Results.BadRequest();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An exception occurred while processing webhook.");
                    return Results.StatusCode(500);
                }
            });

            return routes;
        }
    }
}
