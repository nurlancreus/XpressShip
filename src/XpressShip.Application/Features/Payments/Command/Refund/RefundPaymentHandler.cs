using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Payments.Command.Refund;
using XpressShip.Application.Responses;
using XpressShip.Domain.Enums;
using XpressShip.Application.DTOs.Mail;
using XpressShip.Application.Abstractions;
using XpressShip.Domain.Abstractions;
using System.Threading;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Payment;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace XpressShip.Application.Features.Payments.Command.Refund
{
    public class RefundPaymentHandler : ICommandHandler<RefundPaymentCommand, string>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IJwtSession _jwtSession;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;

        public RefundPaymentHandler(IApiClientSession apiClientSession, IJwtSession jwtSession, IPaymentRepository paymentRepository, IPaymentService paymentService)
        {
            _apiClientSession = apiClientSession;
            _jwtSession = jwtSession;
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
        }

        public async Task<Result<string>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.Table
                                .Include(p => p.Shipment)
                                    .ThenInclude(s => s.ApiClient)
                                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (payment is null) return Result<string>.Failure(Error.NotFoundError(nameof(payment)));

            if (payment.Shipment.ApiClient is ApiClient apiClient)
            {
                var keysResult = _apiClientSession.GetClientApiAndSecretKey();

                if (!keysResult.IsSuccess) return Result<string>.Failure(keysResult.Error);

                if (apiClient.ApiKey != keysResult.Value.apiKey || apiClient.SecretKey != keysResult.Value.secretKey) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to refund the payment"));
            }
            else if (payment.Shipment.SenderId is string senderId)
            {
                var userIdResult = _jwtSession.GetUserId();

                if (!userIdResult.IsSuccess) return Result<string>.Failure(userIdResult.Error);

                if (senderId != userIdResult.Value) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to refund the payment"));
            }
            else
            {
                var isAdminResult = _jwtSession.IsAdminAuth();

                if (!isAdminResult.IsSuccess) return Result<string>.Failure(isAdminResult.Error);
            }

            if (payment.TransactionId != request.TransactionId) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to refund the payment"));

            if (payment.Status == PaymentStatus.Refunded) return Result<string>.Failure(Error.ConflictError("Payment is already refunded"));

            bool isRefunded = await _paymentService.RefundPaymentAsync(payment, cancellationToken);

            if (!isRefunded) return Result<string>.Failure(Error.UnexpectedError("Could not refund the payment"));

            return Result<string>.Success(payment.TransactionId);
        }
    }
}
