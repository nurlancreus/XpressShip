using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Features.Rates.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Rates.Queries.GetRateByShipment
{
    public class GetRateByShipmentHandler : IRequestHandler<GetRateByShipmentQuery, ResponseWithData<ShipmentRateDTO>>
    {
        private readonly IShipmentRepository _shipmentRepository;

        public GetRateByShipmentHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        public async Task<ResponseWithData<ShipmentRateDTO>> Handle(GetRateByShipmentQuery request, CancellationToken cancellationToken)
        {
            var shippingRate = await _shipmentRepository.Table
                .Include(s => s.Rate)
                .Select(s => s.Rate)
                .FirstOrDefaultAsync(s => s.Id == request.ShipmentId, cancellationToken);

            if (shippingRate is null)
            {
                return new ResponseWithData<ShipmentRateDTO>
                {
                    IsSuccess = false,
                    Message = "Shipping rate not found"
                };
            }

            var dto = new ShipmentRateDTO(shippingRate);

            return new ResponseWithData<ShipmentRateDTO>
            {
                IsSuccess = true,
                Message = "Shipping rate retrieved",
                Data = dto
            };
        }
    }
}
