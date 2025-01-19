using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Payment;
using XpressShip.Application.Responses;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Payments.Command.Cancel
{
    public class CancelPaymentHandler : IRequestHandler<CancelPaymentCommand, ResponseWithData<string>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public CancelPaymentHandler(IPaymentRepository paymentRepository, IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<string>> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (payment is null) throw new Exception("Payment is not found");

            if (payment.TransactionId != request.TransactionId) throw new UnauthorizedAccessException("You can not cancel this payment");

            if (payment.Status == PaymentStatus.Canceled) throw new Exception("Payment is already canceled");

            bool isCanceled = await _paymentService.CancelPaymentAsync(payment, cancellationToken);

            if (!isCanceled) throw new Exception("Could not cancel the payment");

            payment.Status = PaymentStatus.Canceled;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResponseWithData<string>
            {
                IsSuccess = true,
                Data = payment.TransactionId,
                Message = "Payment Canceled Successfully!"
            };
        }
    }
}
