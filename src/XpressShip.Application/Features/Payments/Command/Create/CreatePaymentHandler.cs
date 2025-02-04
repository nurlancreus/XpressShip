using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Payment;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Users;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Payments.Command.Create
{
    public class CreatePaymentHandler : ICommandHandler<CreatePaymentCommand, PaymentDTO>
    {
        private readonly IJwtSession _jwtSession;
        private readonly IApiClientSession _apiClientSession;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePaymentHandler(IJwtSession jwtSession, IApiClientSession apiClientSession, IShipmentRepository shipmentRepository, IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _jwtSession = jwtSession;
            _apiClientSession = apiClientSession;
            _shipmentRepository = shipmentRepository;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaymentDTO>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.DestinationAddress)
                                    .ThenInclude(a => a.City)
                                        .ThenInclude(c => c.Country)
                                .Include(s => s.OriginAddress)
                                    .ThenInclude(a => a!.City)
                                        .ThenInclude(c => c.Country)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                                        .ThenInclude(a => a.City)
                                            .ThenInclude(c => c.Country)
                                .Include(s => s.Sender)
                                    .ThenInclude(c => c!.Address)
                                        .ThenInclude(a => a.City)
                                            .ThenInclude(c => c.Country)
                                .Include(s => s.Payment)
                                .FirstOrDefaultAsync(s => s.Id == request.ShipmentId, cancellationToken);

            if (shipment is null) return Result<PaymentDTO>.Failure(Error.NotFoundError("Shipment is not found"));

            if (shipment.ApiClient is ApiClient apiClient)
            {
                var clientIdResult = _apiClientSession.GetClientId();

                if (clientIdResult.IsFailure) return Result<PaymentDTO>.Failure(clientIdResult.Error);

                if (apiClient.Id != clientIdResult.Value)
                {
                    return Result<PaymentDTO>.Failure(Error.UnauthorizedError("You are not authorized to create the payment"));
                }

            }
            else if (shipment.Sender is Sender sender)
            {
                var userIdResult = _jwtSession.GetUserId();

                if (userIdResult.IsFailure) return Result<PaymentDTO>.Failure(userIdResult.Error);

                if (sender.Id != userIdResult.Value)
                {
                    return Result<PaymentDTO>.Failure(Error.UnauthorizedError("You are not authorized to create the payment"));
                }
            } else return Result<PaymentDTO>.Failure(Error.UnauthorizedError("You are not authorized to create the payment"));

            if (shipment.Payment?.TransactionId is not null)
                return Result<PaymentDTO>.Failure(Error.ConflictError("Payment already processed."));

            if (Enum.TryParse<PaymentMethod>(request.Method, true, out var method)) return Result<PaymentDTO>.Failure(Error.BadRequestError());

            if (Enum.TryParse<PaymentCurrency>(request.Currency, true, out var currency)) return Result<PaymentDTO>.Failure(Error.BadRequestError());

            shipment.Payment ??= Payment.Create(method, currency);

            shipment.Payment.TransactionId = await _paymentService.CreatePaymentAsync(shipment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<PaymentDTO>.Success(new PaymentDTO(shipment.Payment));
        }
    }
}
