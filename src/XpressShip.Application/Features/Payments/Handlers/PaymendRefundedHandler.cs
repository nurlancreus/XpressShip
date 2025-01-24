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

namespace XpressShip.Application.Features.Payments.Handlers
{
    public class PaymentRefundedHandler : INotificationHandler<PaymentRefundedNotification>
    {
        private readonly IEmailService _emailService;
        private readonly IPaymentMailTemplatesService _paymentMailTemplatesService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentHubService _paymentHubService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentRefundedHandler(IEmailService emailService, IPaymentMailTemplatesService paymentMailTemplatesService, IPaymentRepository paymentRepository, IPaymentHubService paymentHubService, IUnitOfWork unitOfWork)
        {
            _emailService = emailService;
            _paymentMailTemplatesService = paymentMailTemplatesService;
            _paymentRepository = paymentRepository;
            _paymentHubService = paymentHubService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(PaymentRefundedNotification notification, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.Table
                                    .Include(p => p.Shipment)
                                        .ThenInclude(s => s.ApiClient)
                                    .FirstOrDefaultAsync(p => p.TransactionId == notification.TransactionId, cancellationToken);

            if (payment is null) throw new Exception("Payment is null");

            payment.MakeRefunded();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var recipientDetails = new RecipientDetailsDTO
            {
                Email = payment.Shipment.ApiClient!.Email,
                Name = payment.Shipment.ApiClient.CompanyName,
            };

            var body = _paymentMailTemplatesService.GeneratePaymentRefundedEmail(payment.TransactionId, recipientDetails.Name, payment.Shipment.Cost, payment.Currency.ToString(), DateTime.UtcNow);

            await _emailService.SendEmailAsync(recipientDetails, "Payment Refunded", body);

            var identifier = payment.Shipment.ApiClient?.ApiKey; // if null then take from sender

            await _paymentHubService.PaymentRefundedMessageAsync(identifier!, $"Payment with transaction ID {payment.TransactionId} is refunded successfully!", UserType.ApiClient, cancellationToken);
        }
    }
}
