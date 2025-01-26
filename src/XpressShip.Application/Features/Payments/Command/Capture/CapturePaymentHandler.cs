using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Payments.Command.Capture;
using XpressShip.Application.Responses;
using XpressShip.Domain.Enums;
using XpressShip.Application.Abstractions;
using XpressShip.Domain.Abstractions;
using System.Threading;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Payment;
using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.Payments.Command.Capture
{
    public class CapturePaymentHandler : ICommandHandler<CapturePaymentCommand, string>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IJwtSession _jwtSession;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;

        public async Task<Result<string>> Handle(CapturePaymentCommand request, CancellationToken cancellationToken)
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

                if (apiClient.ApiKey != keysResult.Value.apiKey || apiClient.SecretKey != keysResult.Value.secretKey) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to capture the payment"));
            }
            else if (payment.Shipment.SenderId is string senderId)
            {
                var userIdResult = _jwtSession.GetUserId();

                if (!userIdResult.IsSuccess) return Result<string>.Failure(userIdResult.Error);

                if (senderId != userIdResult.Value) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to capture the payment"));
            }

            if (payment.TransactionId != request.TransactionId) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to capture the payment"));

            if (payment.Status == PaymentStatus.Success) return Result<string>.Failure(Error.ConflictError("Payment is already captured"));

            bool isCaptured = await _paymentService.CapturePaymentAsync(payment, cancellationToken);

            if (!isCaptured) return Result<string>.Failure(Error.UnexpectedError("Could not capture the payment"));

            return Result<string>.Success(payment.TransactionId);
        }
    }
}
