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

namespace XpressShip.Application.Features.Payments.Command.Capture
{
    public class CapturePaymentHandler : ICommandHandler<CapturePaymentCommand, string>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;

        public CapturePaymentHandler(IPaymentRepository paymentRepository, IPaymentService paymentService)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
        }

        public async Task<Result<string>> Handle(CapturePaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (payment is null) return Result<string>.Failure(Error.NotFoundError(nameof(payment)));

            if (payment.TransactionId != request.TransactionId) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to capture the payment"));

            if (payment.Status == PaymentStatus.Success) return Result<string>.Failure(Error.ConflictError("Payment is already captured"));

            bool isCaptured = await _paymentService.CapturePaymentAsync(payment, cancellationToken);

            if (!isCaptured) return Result<string>.Failure(Error.UnexpectedError("Could not capture the payment"));

            return Result<string>.Success(payment.TransactionId);
        }
    }
}
