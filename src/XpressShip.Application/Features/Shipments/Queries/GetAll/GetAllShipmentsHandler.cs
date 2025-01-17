using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Responses;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Extensions;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.Shipments.Queries.GetAll
{
    public class GetAllShipmentsHandler : IRequestHandler<GetAllShipmentsQuery, ResponseWithData<IEnumerable<ShipmentDTO>>>
    {
        private readonly IShipmentRepository _shipmentRepository;

        public GetAllShipmentsHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        public async Task<ResponseWithData<IEnumerable<ShipmentDTO>>> Handle(GetAllShipmentsQuery request, CancellationToken cancellationToken)
        {
            var shipments = _shipmentRepository.Table
                .Include(s => s.Rate)
                .Include(s => s.OriginAddress)
                .Include(s => s.DestinationAddress)
                .Include(s => s.ApiClient)
                    .ThenInclude(c => c.Address)
                .AsNoTracking()
                .AsQueryable();

            // Filter by Status
            if (!string.IsNullOrEmpty(request.Status))
            {
                var shipmentStatus = request.Status.EnsureEnumValueDefined<ShipmentStatus>();
                shipments = shipments.Where(s => s.Status == shipmentStatus);
            }

            // Filter by Client ID
            if (request.ClientId.HasValue)
            {
                shipments = shipments.Where(s => s.ApiClientId == request.ClientId);
            }

            // Filter by Origin Location
            if (request.OriginCountry is string originCountry)
            {
                if (request.OriginCity is string originCity)
                {
                    var isValid = IValidationService.ValidateCountryAndCities(originCountry, [originCity], false);
                    if (isValid)
                    {
                        shipments = shipments.Where(s =>
                            (s.OriginAddress ?? s.ApiClient.Address).Country == originCountry &&
                            (s.OriginAddress ?? s.ApiClient.Address).City == originCity);
                    }
                }
                else
                {
                    var isValid = IValidationService.ValidateCountryAndCities(originCountry, null, false);
                    if (isValid)
                    {
                        shipments = shipments.Where(s => (s.OriginAddress ?? s.ApiClient.Address).Country == originCountry);
                    }
                }
            }

            // Filter by Destination Location
            if (request.DestinationCountry is string destinationCountry)
            {
                if (request.DestinationCity is string destinationCity)
                {
                    var isValid = IValidationService.ValidateCountryAndCities(destinationCountry, [destinationCity], false);
                    if (isValid)
                    {
                        shipments = shipments.Where(s =>
                            s.DestinationAddress.Country == destinationCountry &&
                            s.DestinationAddress.City == destinationCity);
                    }
                }
                else
                {
                    var isValid = IValidationService.ValidateCountryAndCities(destinationCountry, null, false);
                    if (isValid)
                    {
                        shipments = shipments.Where(s => s.DestinationAddress.Country == destinationCountry);
                    }
                }
            }

            var dtos = await shipments
                .Select(s => new ShipmentDTO(s))
                .ToListAsync(cancellationToken);

            return new ResponseWithData<IEnumerable<ShipmentDTO>>
            {
                IsSuccess = true,
                Data = dtos,
                Message = "Shipments retrieved successfully!"
            };
        }
    }
}
