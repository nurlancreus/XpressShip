using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XpressShip.Application.DTOs.Mail;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Mail;
using XpressShip.Application.Interfaces.Services.Mail.Template;
using XpressShip.Application.Responses;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Exceptions;
using XpressShip.Domain.Extensions;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateStatus
{
    public class UpdateShipmentStatusHandler : IRequestHandler<UpdateShipmentStatusCommand, ResponseWithData<ShipmentDTO>>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentMailTemplatesService _shipmentMailTemplatesService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateShipmentStatusHandler(
            IShipmentRepository shipmentRepository,
            IShipmentMailTemplatesService shipmentMailTemplatesService,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
        {
            _shipmentRepository = shipmentRepository;
            _shipmentMailTemplatesService = shipmentMailTemplatesService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<ShipmentDTO>> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.Rate)
                                .Include(s => s.OriginAddress)
                                .Include(s => s.DestinationAddress)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c.Address)
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment is null) throw new ValidationException("Shipment not found.");

            var newStatus = request.Status.EnsureEnumValueDefined<ShipmentStatus>();

            string message = "";
            string subject = "";
            string body = "";
            RecipientDetailsDTO recipientDetails = new();

            switch (newStatus)
            {
                case ShipmentStatus.Pending:
                    shipment.MakePending();

                    recipientDetails = new RecipientDetailsDTO
                    {
                        Email = shipment.ApiClient!.Email,
                        Name = shipment.ApiClient!.CompanyName
                    };
                    body = _shipmentMailTemplatesService.GenerateShipmentConfirmationEmail(
                        shipment.TrackingNumber, recipientDetails.Name, shipment.EstimatedDate);
                    subject = "Shipment Processing - Confirmation";
                    message = "Shipment is Processing!";
                    break;

                case ShipmentStatus.Delivered:
                    shipment.MakeDelivered();

                    recipientDetails = new RecipientDetailsDTO
                    {
                        Email = shipment.ApiClient!.Email,
                        Name = shipment.ApiClient!.CompanyName
                    };
                    body = _shipmentMailTemplatesService.GenerateShipmentDeliveredEmail(
                        shipment.TrackingNumber, recipientDetails.Name, DateTime.UtcNow);
                    subject = "Shipment Delivered";
                    message = "Shipment Delivered Successfully!";
                    break;

                case ShipmentStatus.Canceled:
                    shipment.MakeCanceled();

                    recipientDetails = new RecipientDetailsDTO
                    {
                        Email = shipment.ApiClient!.Email,
                        Name = shipment.ApiClient!.CompanyName
                    };
                    body = _shipmentMailTemplatesService.GenerateShipmentCanceledEmail(
                        shipment.TrackingNumber, recipientDetails.Name);
                    subject = "Shipment Canceled";
                    message = "Shipment Canceled Successfully!";
                    break;

                case ShipmentStatus.Shipped:
                    shipment.MakeShipped();

                    recipientDetails = new RecipientDetailsDTO
                    {
                        Email = shipment.ApiClient!.Email,
                        Name = shipment.ApiClient!.CompanyName
                    };
                    body = _shipmentMailTemplatesService.GenerateShipmentConfirmationEmail(
                        shipment.TrackingNumber, recipientDetails.Name, shipment.EstimatedDate);
                    subject = "Shipment Shipped";
                    message = "Shipment Shipped Successfully!";
                    break;

                case ShipmentStatus.Failed:
                    shipment.MakeFailed();

                    recipientDetails = new RecipientDetailsDTO
                    {
                        Email = shipment.ApiClient!.Email,
                        Name = shipment.ApiClient!.CompanyName
                    };
                    body = _shipmentMailTemplatesService.GenerateShipmentFailedEmail(
                        shipment.TrackingNumber, recipientDetails.Name);
                    subject = "Shipment Failed";
                    message = "Shipment Failed!";
                    break;

                default:
                    throw new ValidationException("Invalid status");
            }

            // Send email
            await _emailService.SendEmailAsync(recipientDetails, body, subject);

            // Commit changes to database
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResponseWithData<ShipmentDTO>
            {
                IsSuccess = true,
                Data = new ShipmentDTO(shipment),
                Message = message
            };
        }
    }
}
