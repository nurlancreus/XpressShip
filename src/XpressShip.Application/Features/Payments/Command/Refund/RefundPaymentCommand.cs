using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.Payments.Command.Refund
{
    public record RefundPaymentCommand : ICommand<string>
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }
}
