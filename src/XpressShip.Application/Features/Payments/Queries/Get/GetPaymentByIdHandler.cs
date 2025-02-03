using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;
using XpressShip.Domain.Exceptions;

namespace XpressShip.Application.Features.Payments.Queries.Get
{
    public class GetPaymentByIdHandler : IQueryHandler<GetPaymentByIdQuery, PaymentDTO>
    {
        private readonly IApiClientSession _apiClientSessionService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IJwtSession _jwtSession;
        public GetPaymentByIdHandler(IApiClientSession apiClientSessionService, IPaymentRepository paymentRepository, IJwtSession jwtSession)
        {
            _apiClientSessionService = apiClientSessionService;
            _paymentRepository = paymentRepository;
            _jwtSession = jwtSession;
        }

        public async Task<Result<PaymentDTO>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.Table
                             .Include(p => p.Shipment)
                                 .ThenInclude(s => s.Sender)
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

            var keysResult = _apiClientSessionService.GetClientApiAndSecretKey();

            if (keysResult.IsSuccess)
            {
                if (payment.Shipment.ApiClient is null || payment.Shipment.ApiClient.ApiKey != keysResult.Value.apiKey || payment.Shipment.ApiClient.SecretKey != keysResult.Value.secretKey)
                    throw new UnauthorizedAccessException("You are not authorized to get the payment details");
            }
            else if (payment.Shipment.Sender is Sender sender)
            {
                var userIdResult = _jwtSession.GetUserId();

                if (userIdResult.IsFailure) return Result<PaymentDTO>.Failure(userIdResult.Error);

                if (sender.Id != userIdResult.Value) return Result<PaymentDTO>.Failure(Error.UnauthorizedError("You are not authorized to get the payment details"));
            }
            else
            {
                var isAdminResult = _jwtSession.IsAdminAuth();

                if (isAdminResult.IsFailure) return Result<PaymentDTO>.Failure(isAdminResult.Error);
            }

            return Result<PaymentDTO>.Success(new PaymentDTO(payment));
        }
    }
}
