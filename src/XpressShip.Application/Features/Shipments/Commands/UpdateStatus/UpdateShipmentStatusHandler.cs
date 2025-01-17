using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Responses;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Exceptions;
using XpressShip.Domain.Extensions;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateStatus
{
    public class UpdateShipmentStatusHandler : IRequestHandler<UpdateShipmentStatusCommand, ResponseWithData<ShipmentDTO>>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateShipmentStatusHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
        {
            _shipmentRepository = shipmentRepository;
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

            shipment.Status = newStatus;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResponseWithData<ShipmentDTO>
            {
                IsSuccess = true,
                Data = new ShipmentDTO(shipment),
                Message = $"Shipment status updated successfully! ({newStatus})"
            };
        }
    }
}
