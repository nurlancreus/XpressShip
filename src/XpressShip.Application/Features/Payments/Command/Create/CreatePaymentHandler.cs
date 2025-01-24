using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Payment;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Extensions;

namespace XpressShip.Application.Features.Payments.Command.Create
{
    public class CreatePaymentHandler : ICommandHandler<CreatePaymentCommand, PaymentDTO>
    {
        private readonly IApiClientSessionService _clientSessionService;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePaymentHandler(IApiClientSessionService clientSessionService, IShipmentRepository shipmentRepository, IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _clientSessionService = clientSessionService;
            _shipmentRepository = shipmentRepository;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaymentDTO>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var keysResult = _clientSessionService.GetClientApiAndSecretKey();

            var shipment = await _shipmentRepository.Table
                                .Include(s => s.DestinationAddress)
                                    .ThenInclude(a => a.City)
                                        .ThenInclude(c => c.Country)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                                .Include(s => s.Payment)
                                .FirstOrDefaultAsync(s => s.Id == request.ShipmentId, cancellationToken);


            if (shipment is null) return Result<PaymentDTO>.Failure(Error.NotFoundError(nameof(shipment)));

            if (keysResult.IsSuccess)
            {
                if (shipment.ApiClient?.ApiKey != keysResult.Value.apiKey || shipment.ApiClient.SecretKey != keysResult.Value.secretKey)
                {
                    return Result<PaymentDTO>.Failure(Error.UnauthorizedError("You are not authorized to update this shipment"));
                }
            }

            if (shipment.Payment?.TransactionId is not null)
             return Result<PaymentDTO>.Failure(Error.ConflictError("Payment already processed."));
            

            var method = request.Method.EnsureEnumValueDefined<PaymentMethod>("method");
            var currency = request.Currency.EnsureEnumValueDefined<PaymentCurrency>("currency");

            shipment.Payment ??= Payment.Create(method, currency);

            shipment.Payment.TransactionId = await _paymentService.CreatePaymentAsync(shipment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<PaymentDTO>.Success(new PaymentDTO(shipment.Payment));
        }
    }
}
