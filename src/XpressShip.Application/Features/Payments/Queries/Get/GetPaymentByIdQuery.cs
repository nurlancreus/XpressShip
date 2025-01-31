﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Payments.Queries.Get
{
    public class GetPaymentByIdQuery : IRequest<ResponseWithData<PaymentDTO>>
    {
        public Guid Id { get; set; }
    }
}
