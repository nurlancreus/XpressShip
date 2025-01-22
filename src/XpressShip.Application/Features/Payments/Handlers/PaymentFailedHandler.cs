using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.DTOs.Mail;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Hubs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Mail;
using XpressShip.Application.Interfaces.Services.Mail.Template;
using XpressShip.Application.Notifications.Payment;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Payments.Handlers
{
    public class PaymentFailedHandler : INotificationHandler<PaymentFailedNotification>
    {
        private readonly IEmailService _emailService;
        private readonly IPaymentMailTemplatesService _paymentMailTemplatesService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentHubService _paymentHubService;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentFailedHandler(IEmailService emailService, IPaymentMailTemplatesService paymentMailTemplatesService, IPaymentRepository paymentRepository, IPaymentHubService paymentHubService, IUnitOfWork unitOfWork)
        {
            _emailService = emailService;
            _paymentMailTemplatesService = paymentMailTemplatesService;
            _paymentRepository = paymentRepository;
            _paymentHubService = paymentHubService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(PaymentFailedNotification notification, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.Table
                                    .Include(p => p.Shipment)
                                        .ThenInclude(s => s.ApiClient)
                                    .FirstOrDefaultAsync(p => p.TransactionId == notification.TransactionId, cancellationToken);

            if (payment is null) throw new Exception("Payment is null");

            payment.MakeFailed();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var recipientDetails = new RecipientDetailsDTO
            {
                Email = payment.Shipment.ApiClient!.Email,
                Name = payment.Shipment.ApiClient.CompanyName,
            };

            var body = _paymentMailTemplatesService.GeneratePaymentFailedEmail(payment.TransactionId, recipientDetails.Name);

            await _emailService.SendEmailAsync(recipientDetails, "Payment Failed", body);

            var identifier = payment.Shipment.ApiClient?.ApiKey; // if null then take from sender

            await _paymentHubService.PaymentFailedMessageAsync(identifier!, $"Payment with transaction ID {payment.TransactionId} is failed!", UserType.ApiClient, cancellationToken);
        }
    }
}
