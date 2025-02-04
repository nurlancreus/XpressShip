using MediatR;
using Microsoft.AspNetCore.Mvc;
using XpressShip.Application.Features.Auth.Admin.Login;
using XpressShip.Application.Features.Auth.Admin.Register;
using XpressShip.Application.Features.Auth.Common.RefreshLogin;
using XpressShip.Application.Features.Auth.Sender.Login;
using XpressShip.Application.Features.Auth.Sender.Register;

namespace XpressShip.API.Endpoints
{
    public static class Auth
    {
        public static IEndpointRouteBuilder RegisterAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            var auth = routes.MapGroup("/api/auth").AllowAnonymous();

            auth.MapPost("admin/register", async ([FromBody] RegisterAdminCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            });

            auth.MapPost("admin/login", async ([FromBody] LoginAdminCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            });

            auth.MapPost("sender/register", async ([FromBody] RegisterSenderCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            });

            auth.MapPost("sender/login", async ([FromBody] LoginSenderCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            });

            auth.MapPost("refresh/login", async ([FromBody] RefreshLoginCommand request, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);

                return result.Match(
                    onSuccess: (value) => Results.Ok(value),
                    onFailure: (error) => error.HandleError(context));
            });

            return routes;
        }
    }
}
