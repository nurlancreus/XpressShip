using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Payments.DTOs;

namespace XpressShip.Application.Features.Payments.Queries.Get
{
    public class GetPaymentByIdQuery : IQuery<PaymentDTO>
    {
        public Guid Id { get; set; }
    }
}
