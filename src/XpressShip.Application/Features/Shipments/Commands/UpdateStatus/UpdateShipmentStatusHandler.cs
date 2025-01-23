using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.DTOs.Mail;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Hubs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Mail;
using XpressShip.Application.Interfaces.Services.Mail.Template;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Exceptions;
using XpressShip.Domain.Extensions;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateStatus
{
    public class UpdateShipmentStatusHandler : ICommandHandler<UpdateShipmentStatusCommand, ShipmentDTO>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentMailTemplatesService _shipmentMailTemplatesService;
        private readonly IEmailService _emailService;
        private readonly IShipmentHubService _shipmentHubService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateShipmentStatusHandler(
            IShipmentRepository shipmentRepository,
            IShipmentMailTemplatesService shipmentMailTemplatesService,
            IEmailService emailService,
            IShipmentHubService shipmentHubService,
            IUnitOfWork unitOfWork)
        {
            _shipmentRepository = shipmentRepository;
            _shipmentMailTemplatesService = shipmentMailTemplatesService;
            _emailService = emailService;
            _shipmentHubService = shipmentHubService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ShipmentDTO>> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.Rate)
                                .Include(s => s.OriginAddress)
                                .Include(s => s.DestinationAddress)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment is null) return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(shipment)));

            UserType user = shipment.ApiClient is not null ? UserType.ApiClient : UserType.Account;

            var identifier = shipment.ApiClient?.ApiKey; // if null then take from sender

            var newStatus = request.Status.EnsureEnumValueDefined<ShipmentStatus>();

            string subject = "";
            string body = "";
            RecipientDetailsDTO? recipientDetails = null;

            switch (newStatus)
            {
                case ShipmentStatus.Pending:
                    shipment.MakePending();

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

                    await _shipmentHubService.ShipmentDeliveredMessageAsync(identifier!, $"Shipment with tracking code ({shipment.TrackingNumber}) is successfully delivered!", user, cancellationToken);

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

                    await _shipmentHubService.ShipmentCanceledMessageAsync(identifier!, $"Shipment with tracking code ({shipment.TrackingNumber}) is canceled!", user, cancellationToken);
                    break;

                case ShipmentStatus.Shipped:
                    shipment.MakeShipped();

                    recipientDetails = new RecipientDetailsDTO
                    {
                        Email = shipment.ApiClient!.Email,
                        Name = shipment.ApiClient!.CompanyName
                    };
                    body = _shipmentMailTemplatesService.GenerateShipmentConfirmationEmail(
                        shipment.TrackingNumber, recipientDetails.Name, (DateTime)shipment.EstimatedDate!);

                    subject = "Shipment Shipped";

                    await _shipmentHubService.ShipmentShippedMessageAsync(identifier!, $"Shipment with tracking code ({shipment.TrackingNumber}) is successfully shipped! Estimated Delivery Date: {(DateTime)shipment.EstimatedDate!:F}", user, cancellationToken);
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

                    await _shipmentHubService.ShipmentFailedMessageAsync(identifier!, $"Shipment with tracking code ({shipment.TrackingNumber}) is unfortunately failed!", user, cancellationToken);
                    break;

                default:
                    return Result<ShipmentDTO>.Failure(Error.UnexpectedError("Invalid Status"));
            }


            if (recipientDetails != null && !string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(subject))
                await _emailService.SendEmailAsync(recipientDetails, body, subject);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ShipmentDTO>.Success(new ShipmentDTO(shipment));
        }
    }
}
