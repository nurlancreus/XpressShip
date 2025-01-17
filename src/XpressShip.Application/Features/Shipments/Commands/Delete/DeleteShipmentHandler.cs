using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Responses;
using XpressShip.Domain.Exceptions;

namespace XpressShip.Application.Features.Shipments.Commands.Delete
{
    public class DeleteShipmentHandler : IRequestHandler<DeleteShipmentCommand, BaseResponse>
    {
        private readonly IClientSessionService _clientSessionService;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteShipmentHandler(IClientSessionService clientSessionService, IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
        {
            _clientSessionService = clientSessionService;
            _shipmentRepository = shipmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.Rate)
                                    .ThenInclude(r => r.Shipments)
                                .Include(s => s.OriginAddress)
                                .Include(s => s.DestinationAddress)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c.Address)
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment is null) throw new ValidationException("Shipment not found.");

            var (apiKey, secretKey) = _clientSessionService.GetClientApiAndSecretKey();

            if (shipment.ApiClient.ApiKey != apiKey || shipment.ApiClient.SecretKey != secretKey)
            {
                throw new UnauthorizedAccessException("You cannot update this shipment");
            }

            _shipmentRepository.Delete(shipment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BaseResponse
            {
                IsSuccess = true,
                Message = "Shipment deleted successfully!"
            };
        }
    }
}
