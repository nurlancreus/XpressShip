using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Payments.Command.Refund;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Payment;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Responses;
using XpressShip.Domain.Enums;
using XpressShip.Application.Interfaces.Services.Mail.Template;
using XpressShip.Application.Interfaces.Services.Mail;
using XpressShip.Application.DTOs.Mail;
using XpressShip.Application.Abstractions;
using XpressShip.Domain.Abstractions;
using System.Threading;

namespace XpressShip.Application.Features.Payments.Command.Refund
{
    public class RefundPaymentHandler : ICommandHandler<RefundPaymentCommand, string>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;

        public RefundPaymentHandler(IPaymentRepository paymentRepository, IPaymentService paymentService)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
        }

        public async Task<Result<string>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (payment is null) return Result<string>.Failure(Error.NotFoundError(nameof(payment)));

            if (payment.TransactionId != request.TransactionId) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to refund the payment"));

            if (payment.Status == PaymentStatus.Refunded) return Result<string>.Failure(Error.ConflictError("Payment is already refunded"));

            bool isRefunded = await _paymentService.RefundPaymentAsync(payment, cancellationToken);

            if (!isRefunded) return Result<string>.Failure(Error.UnexpectedError("Could not refunded the payment"));

            return Result<string>.Success(payment.TransactionId);
        }
    }
}
