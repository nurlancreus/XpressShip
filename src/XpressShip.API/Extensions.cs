using XpressShip.Domain.Abstractions;

namespace XpressShip.API
{
    public static class Extensions
    {
        public static IResult HandleError(this Error error, HttpContext httpContext)
        {
            return Results.Problem(error.Message, $"{httpContext.Request.Method} {httpContext.Request.Path}", (int)error.StatusCode, error.Title, error.Type);
        }
    }
}
