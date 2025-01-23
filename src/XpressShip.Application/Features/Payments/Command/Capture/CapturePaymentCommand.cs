using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Payments.Command.Capture
{
    public record CapturePaymentCommand : ICommand<string>
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }
}
