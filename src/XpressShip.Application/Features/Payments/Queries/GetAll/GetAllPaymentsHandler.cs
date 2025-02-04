using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Features.Payments.Queries.GetAll
{
    public class GetAllPaymentsHandler : IQueryHandler<GetAllPaymentsQuery, IEnumerable<PaymentDTO>>
    {
        private readonly IJwtSession _jwtSession;
        private readonly IPaymentRepository _paymentRepository;

        public GetAllPaymentsHandler(IPaymentRepository paymentRepository, IJwtSession jwtSession)
        {
            _paymentRepository = paymentRepository;
            _jwtSession = jwtSession;
        }

        public async Task<Result<IEnumerable<PaymentDTO>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        {
            var isAdminResult = _jwtSession.IsAdminAuth();

            if (isAdminResult.IsFailure) return Result<IEnumerable<PaymentDTO>>.Failure(isAdminResult.Error);

            var payments = _paymentRepository.Table
                                .Include(p => p.Shipment)
                                     .ThenInclude(s => s.ApiClient)
                                        .ThenInclude(c => c!.Address)
                                            .ThenInclude(a => a.City)
                                                .ThenInclude(c => c.Country)
                                .Include(p => p.Shipment)
                                     .ThenInclude(s => s.Sender)
                                        .ThenInclude(c => c!.Address)
                                            .ThenInclude(a => a.City)
                                                .ThenInclude(c => c.Country)
                                 .Include(p => p.Shipment)
                                    .ThenInclude(s => s.OriginAddress)
                                        .ThenInclude(a => a!.City)
                                            .ThenInclude(c => c.Country)
                                 .Include(p => p.Shipment)
                                    .ThenInclude(s => s.DestinationAddress)
                                        .ThenInclude(a => a.City)
                                            .ThenInclude(c => c.Country)
                                 .AsNoTracking();

            if (request.ClientId is Guid clientId)
            {
                payments = payments.Where(p => p.Shipment.ApiClient != null && p.Shipment.ApiClient.Id == clientId);
            }

            if (request.SenderId is string senderId)
            {
                payments = payments.Where(p => p.Shipment.ApiClient != null && p.Shipment.SenderId == senderId);
            }

            var dtos = await payments.Select(p => new PaymentDTO(p)).ToListAsync(cancellationToken);

            return Result<IEnumerable<PaymentDTO>>.Success(dtos);
        }
    }
}
