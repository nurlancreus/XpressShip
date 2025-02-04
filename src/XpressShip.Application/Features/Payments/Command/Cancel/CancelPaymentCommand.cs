using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.Payments.Command.Cancel
{
    public record CancelPaymentCommand : ICommand<string>
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }
}
