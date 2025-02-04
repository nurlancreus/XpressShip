using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.Payments.Queries.Get
{
    public class GetPaymentByIdHandler : IQueryHandler<GetPaymentByIdQuery, PaymentDTO>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IJwtSession _jwtSession;
        public GetPaymentByIdHandler(IApiClientSession apiClientSession, IPaymentRepository paymentRepository, IJwtSession jwtSession)
        {
            _apiClientSession = apiClientSession;
            _paymentRepository = paymentRepository;
            _jwtSession = jwtSession;
        }

        public async Task<Result<PaymentDTO>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.Table
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
                             .AsNoTracking()
                             .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (payment is null) return Result<PaymentDTO>.Failure(Error.NotFoundError("Payment is not found"));

            var isAdminResult = _jwtSession.IsAdminAuth();

            if (isAdminResult.IsFailure)
            {
                if (payment.Shipment.ApiClient is ApiClient apiClient)
                {
                    var clientIdResult = _apiClientSession.GetClientId();

                    if (clientIdResult.IsFailure) return Result<PaymentDTO>.Failure(clientIdResult.Error);

                    if (apiClient.Id != clientIdResult.Value) return Result<PaymentDTO>.Failure(Error.UnauthorizedError("You are not authorized to get the payment details"));
                }
                else if (payment.Shipment.Sender is Sender sender)
                {
                    var userIdResult = _jwtSession.GetUserId();

                    if (userIdResult.IsFailure) return Result<PaymentDTO>.Failure(userIdResult.Error);

                    if (sender.Id != userIdResult.Value) return Result<PaymentDTO>.Failure(Error.UnauthorizedError("You are not authorized to get the payment details"));
                }
                else return Result<PaymentDTO>.Failure(Error.UnauthorizedError("You are not authorized to get the payment details"));
            }

            return Result<PaymentDTO>.Success(new PaymentDTO(payment));
        }
    }
}
