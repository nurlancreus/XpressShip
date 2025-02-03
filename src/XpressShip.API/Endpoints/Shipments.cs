
using MediatR;
using Microsoft.AspNetCore.Mvc;
using XpressShip.Application.Features.Shipments.Commands.Create.ByApiClient;
using XpressShip.Application.Features.Shipments.Commands.Delete;
using XpressShip.Application.Features.Shipments.Commands.UpdateDetails;
using XpressShip.Application.Features.Shipments.Commands.UpdateStatus;
using XpressShip.Application.Features.Shipments.Queries.GetAll;
using XpressShip.Application.Features.Shipments.Queries.GetById;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.API.Endpoints
{
    public static class Shipments
    {
        public static void RegisterShipmentEndpoints(this IEndpointRouteBuilder routes)
        {
            var shipments = routes.MapGroup("/api/shipments");

            shipments.MapGet("", async ([FromQuery] GetAllShipmentsQuery request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            });

            shipments.MapGet("/{id:guid}", async (Guid id, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {

                var result = await sender.Send(new GetShipmentByIdQuery { Id = id }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context)
                );
            });

            shipments.MapPatch("details", async ([FromBody] UpdateShipmentDetailsCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);

                    return result.Match(
                        onSuccess: (value) => Results.Ok(value),
                        onFailure: error => error.HandleError(context));
                });

            shipments.MapPatch("status", async ([FromBody] UpdateShipmentStatusCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);

                    return result.Match(
                        onSuccess: (value) => Results.Ok(value),
                        onFailure: error => error.HandleError(context));
                });

            shipments.MapDelete("/{id:guid}", async (Guid id, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {

                var result = await sender.Send(new DeleteShipmentCommand { Id = id }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context)
                );
            });
        }
    }
}
