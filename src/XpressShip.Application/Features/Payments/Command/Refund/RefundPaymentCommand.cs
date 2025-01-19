﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Payments.Command.Refund
{
    public record RefundPaymentCommand : IRequest<ResponseWithData<string>>
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }
}
