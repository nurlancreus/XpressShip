using XpressShip.Domain.Abstractions;

namespace XpressShip.API
{
    public static class Extensions
    {
        public static IResult Match<TValue>(
            this Result<TValue> result,
            Func<TValue, IResult> onSuccess,
            Func<Error, IResult> onFailure) where TValue : notnull
        {
            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
        }
        public static IResult HandleError(this Error error, HttpContext httpContext)
        {
            Dictionary<string, object?> extensions = [];

            if (error.Type == ErrorType.Validation)
                extensions.Add("ValidationErrors", error.ValidationErrorMessages);

            return Results.Problem(error.Message, $"{httpContext.Request.Method} {httpContext.Request.Path}", (int)error.StatusCode, error.Title, error.Title, extensions);
        }
    }
}
