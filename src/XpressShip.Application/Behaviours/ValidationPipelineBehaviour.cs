using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exceptions = XpressShip.Domain.Exceptions;

namespace XpressShip.Application.Behaviours
{
    public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : notnull, IRequest<TResponse>
         where TResponse : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return await next();

            var validationContext = new ValidationContext<TRequest>(request);
            var validationResponse = await Task.WhenAll(_validators.Select(x => x.ValidateAsync(validationContext, cancellationToken)));

            var validationErrors = validationResponse
                .SelectMany(x => x.Errors)
                .Where(e => e != null)
                //.Select(x => x.ErrorMessage)
                .ToList();

            // If there are any failures, throw a ValidationException with custom error messages
            if (validationErrors.Count != 0)
            {
                var validationFailures = validationErrors
                    .Select(f => new ValidationFailure(f.PropertyName, f.ErrorMessage))
                    .ToList();

                throw new ValidationException(validationFailures);
            }

            return await next();
        }
    }
}
