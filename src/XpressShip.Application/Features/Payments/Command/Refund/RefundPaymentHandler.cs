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

namespace XpressShip.Application.Features.Payments.Command.Refund
{
    public class RefundPaymentHandler : IRequestHandler<RefundPaymentCommand, ResponseWithData<string>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public RefundPaymentHandler(IPaymentRepository paymentRepository, IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<string>> Handle(RefundPaymentCommand request, CancellationToken RefundlationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id, true, RefundlationToken);

            if (payment is null) throw new Exception("Payment is not found");

            if (payment.TransactionId != request.TransactionId) throw new UnauthorizedAccessException("You can not refund this payment");

            if (payment.Status == PaymentStatus.Refunded) throw new Exception("Payment is already refunded");

            bool isRefunded = await _paymentService.RefundPaymentAsync(payment, RefundlationToken);

            if (!isRefunded) throw new Exception("Could not refund the payment");

            payment.Status = PaymentStatus.Refunded;

            await _unitOfWork.SaveChangesAsync(RefundlationToken);

            return new ResponseWithData<string>
            {
                IsSuccess = true,
                Data = payment.TransactionId,
                Message = "Payment Refunded Successfully!"
            };
        }
    }
}
