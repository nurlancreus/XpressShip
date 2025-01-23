using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities.Base;

namespace XpressShip.Domain.Abstractions
{
    public static class ResultExtensions
    {
        public static T Match<TValue, T>(
            this Result<TValue> result,
            Func<TValue, T> onSuccess,
            Func<Error, T> onFailure) where TValue : notnull
        {
            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
        }

    }
}
