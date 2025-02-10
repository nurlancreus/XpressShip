using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
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

                return CreateFailureResult(domainError);
            }
            catch (XpressShipException ex)
            {
                // Handle application-specific exceptions
                var error = Error.BadRequestError(ex.Message);
                return CreateFailureResult(error);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions by returning a Error.Unexpected
                var error = Error.UnexpectedError($"An unexpected error occurred: {ex.Message}");
                return CreateFailureResult(error);
            }
        }

        private static TResponse CreateFailureResult(Error error)
        {
            // Use reflection to create a Result<T> object dynamically
            var resultType = typeof(Result<>).MakeGenericType(typeof(TResponse).GenericTypeArguments[0]);
            var failureMethod = resultType.GetMethod("Failure", [typeof(Error)]);

            if (failureMethod is not null)
            {
                var failureResult = failureMethod.Invoke(null, [error]);

                if (failureResult is not null) return (TResponse)failureResult;
            }

            throw new InvalidOperationException($"Failed to create a failure result for type {typeof(TResponse)}.");
        }
    }
}