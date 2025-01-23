using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Abstractions
{
    public interface ICommand<TResponse> : IRequestBase, IRequest<Result<TResponse>>
        where TResponse : notnull
    { }

    public interface ICommand : IRequestBase, IRequest<Result<Unit>> { }
}
