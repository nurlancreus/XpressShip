using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Payment;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Payments.Command.Cancel
{
    public class CancelPaymentHandler : ICommandHandler<CancelPaymentCommand, string>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;

        public CancelPaymentHandler(IPaymentRepository paymentRepository, IPaymentService paymentService)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
        }

        public async Task<Result<string>> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (payment is null) return Result<string>.Failure(Error.NotFoundError(nameof(payment)));

            if (payment.TransactionId != request.TransactionId) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to cancel the payment"));

            if (payment.Status == PaymentStatus.Canceled) return Result<string>.Failure(Error.ConflictError("Payment is already canceled"));

            bool isCanceled = await _paymentService.CancelPaymentAsync(payment, cancellationToken);

            if (!isCanceled) return Result<string>.Failure(Error.UnexpectedError("Could not cancel the payment"));

            return Result<string>.Success(payment.TransactionId);
        }
    }
}
