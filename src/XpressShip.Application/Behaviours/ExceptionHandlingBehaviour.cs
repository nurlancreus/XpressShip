using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Exceptions;

namespace XpressShip.Application.Behaviours
{
    public class ExceptionHandlingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                // Proceed to the next behavior or actual handler
                return await next();
            }
            catch (ValidationException ex)
            {
                // Handle validation exceptions by returning a DomainError.Validation
                var errors = ex.Errors;

                var validationErrors = errors.Select(e => new KeyValuePair<string, string>(e.PropertyName, e.ErrorMessage));

                var domainError = Error.ValidationError(ex.Message, validationErrors);
                var failureResult = Result<Guid>.Failure(domainError);

                if (failureResult is TResponse response)
                {
                    return response;
                }

                throw new InvalidCastException("Failed to cast validation error result to the expected TResponse type.");
            }

            catch (XpressShipException ex)
            {
                // Handle application-specific exceptions
                var error = Error.BadRequestError(ex.Message);
                var failureResult = Result<Guid>.Failure(error);

                if (failureResult is TResponse response)
                {
                    return response;
                }

                throw new InvalidCastException("Failed to cast bad request error result to the expected TResponse type.");
            }

            catch (Exception ex)
            {
                // Handle unexpected exceptions by returning a Error.Unexpected
                var error = Error.UnexpectedError($"An unexpected error occurred: {ex.Message}");

                var failureResult = Result<Guid>.Failure(error);

                if (failureResult is TResponse response)
                {
                    return response;
                }

                throw new InvalidCastException("Failed to cast unexpected error result to the expected TResponse type.");
            }
        }
    }
}
