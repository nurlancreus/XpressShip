using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.Shipments.Commands.Delete
{
    public record DeleteShipmentCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}
