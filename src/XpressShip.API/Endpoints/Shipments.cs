
namespace XpressShip.API.Endpoints
{
    public static class Shipments
    {
        public static void RegisterShipmentEndpoints(this IEndpointRouteBuilder routes)
        {
            var shipments = routes.MapGroup("/api/v1/shipments");

            shipments.MapGet("", () => { return Results.NotFound(); });

            shipments.MapGet("/{id}", (int id) => { return Results.BadRequest(); });


            shipments.MapPost("", () => { return Results.Unauthorized(); });


            shipments.MapPut("/{id}", (int id) =>
            {
                var errors = new Dictionary<string, string[]> { { "HHOHIO", ["HOHHHHHHHHHHH", "hiiiiiii"] } };

                return Results.ValidationProblem(errors);
            });

            shipments.MapDelete("/{id}", (int id) => { });
        }
    }
}
