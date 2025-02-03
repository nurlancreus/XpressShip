

using MediatR;
using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.ApiClients.Commands.Delete
{
    public record DeleteApiClientCommand : ICommand<Unit>
    {
        public Guid Id { get; set; }
    }
}
