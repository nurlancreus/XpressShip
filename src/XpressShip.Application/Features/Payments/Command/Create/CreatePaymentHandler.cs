using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Payment;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Responses;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Extensions;

namespace XpressShip.Application.Features.Payments.Command.Create
{
    public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, ResponseWithData<PaymentDTO>>
    {
        private readonly IClientSessionService _clientSessionService;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePaymentHandler(IClientSessionService clientSessionService, IShipmentRepository shipmentRepository, IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _clientSessionService = clientSessionService;
            _shipmentRepository = shipmentRepository;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<PaymentDTO>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var keys = _clientSessionService.GetClientApiAndSecretKey();

            var shipment = await _shipmentRepository.Table
                                .Include(s => s.DestinationAddress)
                                    .ThenInclude(a => a.City)
                                        .ThenInclude(c => c.Country)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                                .Include(s => s.Payment)
                                .FirstOrDefaultAsync(s => s.Id == request.ShipmentId, cancellationToken);


            if (shipment is null) throw new ValidationException("Shipment not found.");

            if (keys is (string apiKey, string secretKey))
            {

                if (shipment.ApiClient?.ApiKey != apiKey || shipment.ApiClient.SecretKey != secretKey)
                {
                    throw new UnauthorizedAccessException("You cannot update this shipment");
                }
            }

            if (shipment.Payment?.TransactionId is not null)
            {
                return new ResponseWithData<PaymentDTO>
                {
                    IsSuccess = true,
                    Data = new PaymentDTO(shipment.Payment),
                    Message = "Payment already processed."
                };
            }

            var method = request.Method.EnsureEnumValueDefined<PaymentMethod>("method");
            var currency = request.Currency.EnsureEnumValueDefined<PaymentCurrency>("currency");

            shipment.Payment ??= Payment.Create(method, currency);

            shipment.Payment.TransactionId = await _paymentService.CreatePaymentAsync(shipment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResponseWithData<PaymentDTO>
            {

                IsSuccess = true,
                Data = new PaymentDTO(shipment.Payment),
                Message = "Payment process created successfully!"
            };
        }
    }
}
