using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Domain.Abstractions
{
    public sealed record Error : IError
    {
        public readonly string Title;
        public readonly string Type;
        public readonly string Message;
        public readonly HttpStatusCode StatusCode;

        public static readonly Error None = new(string.Empty, HttpStatusCode.OK, string.Empty);

        private Error(string title, HttpStatusCode statusCode, string message)
        {
            Title = title;
            Type = title;
            StatusCode = statusCode;
            Message = message;
        }

        public static Error RegisterError(string message = "You could not register. Wrong credentials")
        {
            return new(nameof(ErrorType.Register), HttpStatusCode.BadRequest, message);
        }
        public static Error LoginError(string message = "You could not login. Wrong credentials")
        {
            return new(nameof(ErrorType.Login), HttpStatusCode.BadRequest, message);
        }
        public static Error NotFoundError(string model = "Entity")
        {
            return new(nameof(ErrorType.NotFound), HttpStatusCode.NotFound, $"{model} Not Found");
        }
        public static Error ValidationError(string message = "Validation error happened")
        {
            return new(nameof(ErrorType.Validation), HttpStatusCode.BadRequest, message);
        }

        public static Error BadRequestError(string message = "Invalid request or parameters.")
        {
            return new(nameof(ErrorType.BadRequest), HttpStatusCode.BadRequest, message);
        }

        public static Error ConflictError(string message = "The data provided conflicts with existing data.")
        {
            return new(nameof(ErrorType.Conflict), HttpStatusCode.Conflict, message);
        }

        public static Error UnauthorizedError(string message = "You're unauthorized")
        {
            return new(nameof(ErrorType.Unauthorized), HttpStatusCode.Unauthorized, message);
        }

        public static Error ForbiddenError(string message = "This operation is forbidden for you")
        {
            return new(nameof(ErrorType.Forbidden), HttpStatusCode.Forbidden, message);
        }

        public static Error UnexpectedError(string message = "Unexpected error happened. Something went wrong")
        {
            return new(nameof(ErrorType.Unexpected), HttpStatusCode.InternalServerError, message);
        }
    }
}
