using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateStatus
{
    public record UpdateShipmentStatusCommand : ICommand<string>
    {
        public Guid? Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
