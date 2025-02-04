using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Payments.DTOs;

namespace XpressShip.Application.Features.Payments.Command.Create
{
    public record CreatePaymentCommand : ICommand<PaymentDTO>
    {
        public Guid ShipmentId { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
    }
}
