using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.ApiClients.Commands.Toggle
{
    public record ToggleApiClientCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}
