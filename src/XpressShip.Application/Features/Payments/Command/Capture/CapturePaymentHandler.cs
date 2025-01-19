using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Payments.Command.Capture;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Payment;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Responses;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Payments.Command.Capture
{
    public class CapturePaymentHandler : IRequestHandler<CapturePaymentCommand, ResponseWithData<string>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public CapturePaymentHandler(IPaymentRepository paymentRepository, IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<string>> Handle(CapturePaymentCommand request, CancellationToken CapturelationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id, true, CapturelationToken);

            if (payment is null) throw new Exception("Payment is not found");

            if (payment.TransactionId != request.TransactionId) throw new UnauthorizedAccessException("You can not capture this payment");

            if (payment.Status == PaymentStatus.Success) throw new Exception("Payment is already captured");

            bool isCaptureed = await _paymentService.CapturePaymentAsync(payment, CapturelationToken);

            if (!isCaptureed) throw new Exception("Could not capture the payment");

            payment.Status = PaymentStatus.Success;

            await _unitOfWork.SaveChangesAsync(CapturelationToken);

            return new ResponseWithData<string>
            {
                IsSuccess = true,
                Data = payment.TransactionId,
                Message = "Payment Captured Successfully!"
            };
        }
    }
}
