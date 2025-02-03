using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.DTOs.Mail;
using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Notifications.Payment;
using XpressShip.Domain.Enums;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Hubs;
using XpressShip.Application.Abstractions.Services.Mail;
using XpressShip.Application.Abstractions.Services.Mail.Template;
using XpressShip.Domain.Exceptions;

namespace XpressShip.Application.Features.Payments.Handlers
{
    public class PaymentCanceledHandler : INotificationHandler<PaymentCanceledNotification>
    {
        private readonly IEmailService _emailService;
        private readonly IPaymentMailTemplatesService _paymentMailTemplatesService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentHubService _paymentHubService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentCanceledHandler(IEmailService emailService, IPaymentMailTemplatesService paymentMailTemplatesService, IPaymentRepository paymentRepository, IPaymentHubService paymentHubService, IUnitOfWork unitOfWork)
        {
            _emailService = emailService;
            _paymentMailTemplatesService = paymentMailTemplatesService;
            _paymentRepository = paymentRepository;
            _paymentHubService = paymentHubService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(PaymentCanceledNotification notification, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.Table
                                    .Include(p => p.Shipment)
                                        .ThenInclude(s => s.ApiClient)
                                    .Include(p => p.Shipment)
                                        .ThenInclude(s => s.Sender)
                                    .FirstOrDefaultAsync(p => p.TransactionId == notification.TransactionId, cancellationToken);

            if (payment is null) throw new XpressShipException("Payment is not found");

            payment.MakeCanceled();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var (initiatorType, initiatorId) = payment.Shipment.GetInitiatorTypeAndId();
            var (name, email) = payment.Shipment.GetRecipient();

            var recipientDetails = new RecipientDetailsDTO
            {
                Email = email,
                Name = name,
            };

            var body = _paymentMailTemplatesService.GeneratePaymentCanceledEmail(payment.TransactionId, recipientDetails.Name);

            await _emailService.SendEmailAsync(recipientDetails, "Payment Canceled", body);

            await _paymentHubService.PaymentCanceledMessageAsync(initiatorId, $"Payment with transaction ID {payment.TransactionId} is canceled!", initiatorType, cancellationToken);
        }
    }
}
