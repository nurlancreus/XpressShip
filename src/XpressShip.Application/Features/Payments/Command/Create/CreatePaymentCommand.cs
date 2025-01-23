using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Payments.Command.Create
{
    public record CreatePaymentCommand : ICommand<PaymentDTO>
    {
        public Guid ShipmentId { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
    }
}
