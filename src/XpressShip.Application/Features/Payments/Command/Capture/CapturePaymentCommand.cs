using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.Payments.Command.Capture
{
    public record CapturePaymentCommand : ICommand<string>
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }
}
