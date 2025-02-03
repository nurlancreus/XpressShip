using MediatR;
using Microsoft.AspNetCore.Mvc;
using XpressShip.API.Attributes;
using XpressShip.Application.Features.ApiClients.Commands.Create;
using XpressShip.Application.Features.ApiClients.Commands.Delete;
using XpressShip.Application.Features.ApiClients.Commands.Toggle;
using XpressShip.Application.Features.ApiClients.Commands.Update;
using XpressShip.Application.Features.ApiClients.Commands.UpdateApiKey;
using XpressShip.Application.Features.ApiClients.Commands.UpdateSecretKey;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Features.ApiClients.Queries.Get;
using XpressShip.Application.Features.ApiClients.Queries.GetAll;
using XpressShip.Application.Features.Shipments.Commands.Create.ByApiClient;
using XpressShip.Domain.Abstractions;

namespace XpressShip.API.Endpoints
{
    public static class Clients
    {
        public static void RegisterApiClientEndpoints(this IEndpointRouteBuilder routes)
        {
            var clients = routes.MapGroup("/api/clients");

            clients.MapGet("", async (ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllApiClientsQuery(), cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: error => error.HandleError(context));
            });

            clients.MapGet("/{id:guid}", async (Guid id, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetApiClientByIdQuery { Id = id }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: error => error.HandleError(context));
            });

            clients
                .MapPost("", async ([FromBody] CreateApiClientCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: error => error.HandleError(context));
            });

            clients.MapPatch("/{id:guid}", async (Guid id, [FromBody] UpdateApiClientCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                request.Id = id;
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                     onSuccess: (value) => Results.Ok(value),
                     onFailure: error => error.HandleError(context));
            }).WithMetadata(new AuthorizeApiClientAttribute());

            clients.MapPatch("/{id:guid}/toggle", async (Guid id, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new ToggleApiClientCommand { Id = id }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: error => error.HandleError(context));
            }).WithMetadata(new AuthorizeApiClientAttribute());

            clients.MapDelete("/{id:guid}", async (Guid id, ISender service, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await service.Send(new DeleteApiClientCommand { Id = id }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: error => Results.Problem());
            }).WithMetadata(new AuthorizeApiClientAttribute());


            clients.MapPatch("/{id:guid}/api-key", async (Guid id, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new UpdateApiKeyCommand { Id = id }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: error => error.HandleError(context));

            }).WithMetadata(new AuthorizeApiClientAttribute());

            clients.MapPatch("/{id:guid}/secret-key", async (Guid id, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new UpdateSecretKeyCommand { Id = id }, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: error => error.HandleError(context));

            }).WithMetadata(new AuthorizeApiClientAttribute());

            clients
                .MapPost("shipments", async ([FromBody] CreateShipmentByApiClientCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);

                    return result.Match(
                        onSuccess: (value) => Results.Ok(value),
                        onFailure: error => error.HandleError(context));
                }).WithMetadata(new AuthorizeApiClientAttribute());
        }
    }
}
