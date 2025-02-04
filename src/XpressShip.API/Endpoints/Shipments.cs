using MediatR;
using Microsoft.AspNetCore.Mvc;
using XpressShip.Application.Features.Shipments.Commands.Delete;
using XpressShip.Application.Features.Shipments.Commands.UpdateDetails;
using XpressShip.Application.Features.Shipments.Commands.UpdateStatus;
using XpressShip.Application.Features.Shipments.Queries.GetAll;
using XpressShip.Application.Features.Shipments.Queries.GetById;
using XpressShip.Application.Features.Shipments.Queries.GetByTrackingNumber;

namespace XpressShip.API.Endpoints
{
    public static class Shipments
    {
        public static IEndpointRouteBuilder RegisterShipmentEndpoints(this IEndpointRouteBuilder routes)
        {
            var shipments = routes.MapGroup("/api/shipments");

            shipments.MapGet("", async ([FromQuery] GetAllShipmentsQuery request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            }).RequireAuthorization(Constants.AdminsPolicy);

            shipments.MapGet("/{id:guid}", async (Guid id, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetShipmentByIdQuery { Id = id }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context)
                );
            }).RequireAuthorization(Constants.RegisteredUsersOrApiClientsPolicy);

            shipments.MapGet("/{trackingNumber:string}", async (string trackingNumber, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetShipmentByTrackingNumberQuery { TrackingNumber = trackingNumber }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context)
                );
            }).RequireAuthorization(Constants.RegisteredUsersOrApiClientsPolicy);

            shipments.MapPatch("details", async ([FromBody] UpdateShipmentDetailsCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);

                    return result.Match(
                        onSuccess: (value) => Results.Ok(value),
                        onFailure: error => error.HandleError(context));
                }).RequireAuthorization(Constants.SendersOrApiClientsPolicy);

            shipments.MapPatch("status", async ([FromBody] UpdateShipmentStatusCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);

                    return result.Match(
                        onSuccess: (value) => Results.Ok(value),
                        onFailure: error => error.HandleError(context));
                }).RequireAuthorization(Constants.AdminsPolicy);

            shipments.MapDelete("/{id:guid}", async (Guid id, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteShipmentCommand { Id = id }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context)
                );
            }).RequireAuthorization(Constants.RegisteredUsersOrApiClientsPolicy);

            return routes;
        }
    }
}
