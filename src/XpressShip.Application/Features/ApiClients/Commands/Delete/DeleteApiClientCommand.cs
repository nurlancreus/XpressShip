

using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.ApiClients.Commands.Delete
{
    public record DeleteApiClientCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
    }
}
