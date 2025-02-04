using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Hubs;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Mail;
using XpressShip.Application.Abstractions.Services.Mail.Template;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.DTOs.Mail;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateStatus
{
    public class UpdateShipmentStatusHandler : ICommandHandler<UpdateShipmentStatusCommand, string>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IJwtSession _jwtSession;
        private readonly IShipmentMailTemplatesService _shipmentMailTemplatesService;
        private readonly IEmailService _emailService;
        private readonly IShipmentHubService _shipmentHubService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateShipmentStatusHandler(
            IShipmentRepository shipmentRepository,
            IJwtSession jwtSession,
            IShipmentMailTemplatesService shipmentMailTemplatesService,
            IEmailService emailService,
            IShipmentHubService shipmentHubService,
            IUnitOfWork unitOfWork)
        {
            _shipmentRepository = shipmentRepository;
            _jwtSession = jwtSession;
            _shipmentMailTemplatesService = shipmentMailTemplatesService;
            _emailService = emailService;
            _shipmentHubService = shipmentHubService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
        {
            var isAdminResult = _jwtSession.IsAdminAuth();

            if (isAdminResult.IsFailure) return Result<string>.Failure(isAdminResult.Error);

            var shipment = await _shipmentRepository.Table
                                .Include(s => s.ApiClient)
                                .Include(s => s.Sender)
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment is null) return Result<string>.Failure(Error.NotFoundError("Shipment is not found"));

            var (initiatorType, initiatorId) = shipment.GetInitiatorTypeAndId();

            if (!Enum.TryParse<ShipmentStatus>(request.Status, true, out var newStatus)) return Result<string>.Failure(Error.BadRequestError("Could not parse the enum"));

            string subject = "";
            string body = "";
            var (name, email) = shipment.GetRecipient();

            var recipientDetails = new RecipientDetailsDTO
            {
                Email = email,
                Name = name,
            };

            switch (newStatus)
            {
                case ShipmentStatus.Pending:
                    shipment.MakePending();
                    recipientDetails = null;

                    break;

                case ShipmentStatus.Delivered:
                    shipment.MakeDelivered();

                    body = _shipmentMailTemplatesService.GenerateShipmentDeliveredEmail(
                        shipment.TrackingNumber, recipientDetails.Name, DateTime.UtcNow);
                    subject = "Shipment Delivered";

                    await _shipmentHubService.ShipmentDeliveredMessageAsync(initiatorId, $"Shipment with tracking code ({shipment.TrackingNumber}) is successfully delivered!", initiatorType, cancellationToken);

                    break;

                case ShipmentStatus.Canceled:
                    shipment.MakeCanceled();

                    body = _shipmentMailTemplatesService.GenerateShipmentCanceledEmail(
                        shipment.TrackingNumber, recipientDetails.Name);
                    subject = "Shipment Canceled";

                    await _shipmentHubService.ShipmentCanceledMessageAsync(initiatorId, $"Shipment with tracking code ({shipment.TrackingNumber}) is canceled!", initiatorType, cancellationToken);
                    break;

                case ShipmentStatus.Shipped:
                    shipment.MakeShipped();

                    body = _shipmentMailTemplatesService.GenerateShipmentConfirmationEmail(
                        shipment.TrackingNumber, recipientDetails.Name, (DateTime)shipment.EstimatedDate!);

                    subject = "Shipment Shipped";

                    await _shipmentHubService.ShipmentShippedMessageAsync(initiatorId, $"Shipment with tracking code ({shipment.TrackingNumber}) is successfully shipped! Estimated Delivery Date: {(DateTime)shipment.EstimatedDate!:F}", initiatorType, cancellationToken);
                    break;

                case ShipmentStatus.Failed:
                    shipment.MakeFailed();

                    body = _shipmentMailTemplatesService.GenerateShipmentFailedEmail(
                        shipment.TrackingNumber, recipientDetails.Name);

                    subject = "Shipment Failed";

                    await _shipmentHubService.ShipmentFailedMessageAsync(initiatorId, $"Shipment with tracking code ({shipment.TrackingNumber}) is unfortunately failed!", initiatorType, cancellationToken);
                    break;

                default:
                    return Result<string>.Failure(Error.UnexpectedError("Invalid Status"));
            }

            if (recipientDetails is not null && !string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(subject))
                await _emailService.SendEmailAsync(recipientDetails, body, subject);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(shipment.TrackingNumber);
        }
    }
}
