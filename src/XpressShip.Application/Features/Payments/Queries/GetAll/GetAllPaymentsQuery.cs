using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Payments.Queries.GetAll
{
    public class GetAllPaymentsQuery : IRequest<ResponseWithData<IEnumerable<PaymentDTO>>>
    {
        public Guid? ClientId { get; set; }

    }
}
