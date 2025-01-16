using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace XpressShip.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        // Constructor to initialize with multiple errors
        public ValidationException(IEnumerable<string> errors)
            : base("Validation failed.")
        {
            Errors = errors?.ToList() ?? [];
        }

        // Constructor to initialize with a single error
        public ValidationException(string error)
            : base(error)
        {
            Errors = [error];
        }

        // Constructor to provide a custom message and an inner exception
        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
            Errors = [inner.Message];
        }
    }
}
