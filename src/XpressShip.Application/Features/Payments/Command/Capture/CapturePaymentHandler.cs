using XpressShip.Domain.Enums;
using XpressShip.Application.Abstractions;
using XpressShip.Domain.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Payment;
using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.Payments.Command.Capture
{
    public class CapturePaymentHandler : ICommandHandler<CapturePaymentCommand, string>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IJwtSession _jwtSession;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;

        public CapturePaymentHandler(IApiClientSession apiClientSession, IJwtSession jwtSession, IPaymentRepository paymentRepository, IPaymentService paymentService)
        {
            _apiClientSession = apiClientSession;
            _jwtSession = jwtSession;
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
        }
        public async Task<Result<string>> Handle(CapturePaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.Table
                     .Include(p => p.Shipment)
                         .ThenInclude(s => s.ApiClient)
                     .Include(p => p.Shipment)
                         .ThenInclude(s => s.Sender)
                     .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (payment is null) return Result<string>.Failure(Error.NotFoundError("Payment is not found"));

            if (payment.Shipment.ApiClient is ApiClient apiClient)
            {
                var clientIdResult = _apiClientSession.GetClientId();

                if (clientIdResult.IsFailure) return Result<string>.Failure(clientIdResult.Error);

                if (apiClient.Id != clientIdResult.Value) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to capture the payment"));
            }
            else if (payment.Shipment.Sender is Sender sender)
            {
                var userIdResult = _jwtSession.GetUserId();

                if (userIdResult.IsFailure) return Result<string>.Failure(userIdResult.Error);

                if (sender.Id != userIdResult.Value) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to capture the payment"));
            }
            else return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to capture the payment"));

            if (payment.TransactionId != request.TransactionId) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to capture the payment"));

            if (payment.Status == PaymentStatus.Success) return Result<string>.Failure(Error.ConflictError("Payment is already captured"));

            bool isCaptured = await _paymentService.CapturePaymentAsync(payment, cancellationToken);

            if (!isCaptured) return Result<string>.Failure(Error.UnexpectedError("Could not capture the payment"));

            return Result<string>.Success(payment.TransactionId);
        }
    }
}
