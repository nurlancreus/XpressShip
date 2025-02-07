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
        public readonly ErrorType Type;
        public readonly string Message;
        public readonly HttpStatusCode StatusCode;
        public readonly IEnumerable<KeyValuePair<string, string>> ValidationErrorMessages;

        public static readonly Error None = new(ErrorType.None, HttpStatusCode.OK, string.Empty);

        private Error(ErrorType type, HttpStatusCode statusCode, string message, IEnumerable<KeyValuePair<string, string>>? validationErrors = null)
        {
            Title = type.ToString();
            Type = type;
            StatusCode = statusCode;
            Message = message;
            ValidationErrorMessages = validationErrors ?? [];
        }

        public static Error RegisterError(string message = "You could not register. Wrong credentials")
        {
            return new(ErrorType.Register, HttpStatusCode.BadRequest, message);
        }
        public static Error LoginError(string message = "You could not login. Wrong credentials")
        {
            return new(ErrorType.Login, HttpStatusCode.BadRequest, message);
        }
        public static Error NotFoundError(string message = "Entity is not found")
        {
            return new(ErrorType.NotFound, HttpStatusCode.NotFound, message);
        }
        public static Error ValidationError(string message = "Validation error happened", IEnumerable<KeyValuePair<string, string>>? validationErrors = null)
        {
            return new(ErrorType.Validation, HttpStatusCode.BadRequest, message, validationErrors);
        }

        public static Error BadRequestError(string message = "Invalid request or parameters.")
        {
            return new(ErrorType.BadRequest, HttpStatusCode.BadRequest, message);
        }

        public static Error ConflictError(string message = "The data provided conflicts with existing data.")
        {
            return new(ErrorType.Conflict, HttpStatusCode.Conflict, message);
        }

        public static Error UnauthorizedError(string message = "You're unauthorized")
        {
            return new(ErrorType.Unauthorized, HttpStatusCode.Unauthorized, message);
        }

        public static Error ForbiddenError(string message = "This operation is forbidden for you")
        {
            return new(ErrorType.Forbidden, HttpStatusCode.Forbidden, message);
        }

        public static Error TokenError(string message = "Token can not be generated")
        {
            return new(ErrorType.Token, HttpStatusCode.Forbidden, message);
        }

        public static Error UnexpectedError(string message = "Unexpected error happened. Something went wrong")
        {
            return new(ErrorType.Unexpected, HttpStatusCode.InternalServerError, message);
        }
    }
}
