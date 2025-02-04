using MediatR;
using Microsoft.AspNetCore.Mvc;
using XpressShip.Application.Features.Payments.Command.Cancel;
using XpressShip.Application.Features.Payments.Command.Capture;
using XpressShip.Application.Features.Payments.Command.Create;
using XpressShip.Application.Features.Payments.Command.Refund;
using XpressShip.Application.Features.Payments.Queries.Get;
using XpressShip.Application.Features.Payments.Queries.GetAll;

namespace XpressShip.API.Endpoints
{
    public static class Payments
    {
        public static IEndpointRouteBuilder RegisterPaymentEndpoints(this IEndpointRouteBuilder routes)
        {
            var payments = routes.MapGroup("api/payments");

            payments.MapGet("", async ([FromQuery] GetAllPaymentsQuery request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            }).RequireAuthorization(Constants.AdminsPolicy);

            payments.MapGet("{id:guid}", async (Guid id, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetPaymentByIdQuery { Id = id }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            }).RequireAuthorization(Constants.RegisteredUsersOrApiClientsPolicy);

            payments.MapPost("", async ([FromBody] CreatePaymentCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            }).RequireAuthorization(Constants.SendersOrApiClientsPolicy);

            payments.MapPatch("capture", async ([FromBody] CapturePaymentCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            }).RequireAuthorization(Constants.SendersOrApiClientsPolicy);

            payments.MapPatch("cancel", async ([FromBody] CancelPaymentCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            }).RequireAuthorization(Constants.RegisteredUsersOrApiClientsPolicy);

            payments.MapPatch("refund", async ([FromBody] RefundPaymentCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            }).RequireAuthorization(Constants.RegisteredUsersOrApiClientsPolicy);

            return routes;
        }
    }
}
