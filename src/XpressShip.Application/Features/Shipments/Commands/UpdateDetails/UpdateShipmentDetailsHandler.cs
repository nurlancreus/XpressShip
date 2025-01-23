using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Interfaces.Services;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Responses;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;
using Microsoft.EntityFrameworkCore;
using XpressShip.Domain.Exceptions;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.DTOs;
using XpressShip.Domain.Extensions;
using XpressShip.Application.Abstractions;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateDetails
{
    public class UpdateShipmentDetailsHandler : ICommandHandler<UpdateShipmentDetailsCommand, ShipmentDTO>
    {
        private readonly IApiClientSessionService _clientSessionService;
        private readonly IAddressValidationService _addressValidationService;
        private readonly ICountryRepository _countryRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentRateRepository _shipmentRateRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateShipmentDetailsHandler(IApiClientSessionService clientSessionService, IAddressValidationService addressValidationService, ICountryRepository countryRepository, IShipmentRepository shipmentRepository, IShipmentRateRepository shipmentRateRepository, IGeoInfoService geoInfoService, IUnitOfWork unitOfWork)
        {
            _clientSessionService = clientSessionService;
            _addressValidationService = addressValidationService;
            _countryRepository = countryRepository;
            _shipmentRepository = shipmentRepository;
            _shipmentRateRepository = shipmentRateRepository;
            _geoInfoService = geoInfoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ShipmentDTO>> Handle(UpdateShipmentDetailsCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.Rate)
                                .Include(s => s.OriginAddress)
                                .Include(s => s.DestinationAddress)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment is null) return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(shipment)));

            if (shipment.ApiClient is not null)
            {
                var keys = _clientSessionService.GetClientApiAndSecretKey();

                if (keys is (string apiKey, string secretKey))
                {
                    if (shipment.ApiClient.ApiKey != apiKey || shipment.ApiClient.SecretKey != secretKey)
                    {
                        return Result<ShipmentDTO>.Failure(Error.UnauthorizedError("You cannot update this shipment"));
                    }
                }
            }

            // Validate shipment rate
            var shipmentRate = shipment.Rate;

            if (request.ShipmentRateId is Guid rateId && shipmentRate.Id != rateId)
            {
                shipmentRate = await _shipmentRateRepository.GetByIdAsync(rateId, true, cancellationToken);

                if (shipmentRate is null) return Result<ShipmentDTO>.Failure(Error.NotFoundError(nameof(shipmentRate)));

                shipment.Rate = shipmentRate;
            }


            if (request.Origin is AddressCommandDTO originAddress)
            {
                // Validate Origin Address
                await _addressValidationService.ValidateCountryCityAndPostalCodeAsync(originAddress.Country, originAddress.City, originAddress.PostalCode, true, cancellationToken);

                var originGeoInfo = await _geoInfoService.GetLocationGeoInfoByNameAsync(originAddress.Country, originAddress.City, cancellationToken);

                shipment.OriginAddress = Address.Create(originAddress.PostalCode, originAddress.Street, originGeoInfo.Latitude, originGeoInfo.Longitude);

                var country = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == originAddress.Country, cancellationToken);

                country = country.EnsureNonNull();

                var city = country.Cities.FirstOrDefault(c => c.Name == originAddress.City);

                city = city.EnsureNonNull();

                shipment.OriginAddress.City = city;
            }

            if (request.Destination is AddressCommandDTO destinationAddress)
            {
                // Validate Destination Address
                await _addressValidationService.ValidateCountryCityAndPostalCodeAsync(destinationAddress.Country, destinationAddress.City, destinationAddress.PostalCode, true, cancellationToken);

                var destinationGeoInfo = await _geoInfoService.GetLocationGeoInfoByNameAsync(destinationAddress.Country, destinationAddress.City, cancellationToken);

                shipment.DestinationAddress = Address.Create(destinationAddress.PostalCode, destinationAddress.Street, destinationGeoInfo.Latitude, destinationGeoInfo.Longitude);

                var country = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == destinationAddress.Country, cancellationToken);

                country = country.EnsureNonNull();

                var city = country.Cities.FirstOrDefault(c => c.Name == destinationAddress.City);

                city = city.EnsureNonNull();

                shipment.DestinationAddress.City = city;
            }

            if (request.Weight is double weight && shipment.Weight != weight)
            {
                shipment.Weight = weight;
            }

            if (request.Dimensions is string dimensions && shipment.Dimensions != dimensions)
            {
                shipment.Dimensions = dimensions;
            }

            if (request.Note is string note && shipment.Note != note)
            {
                shipment.Note = note;
            }

            if (request.Method is string method)
            {
                var shipmentMethod = method.EnsureEnumValueDefined<ShipmentMethod>();

                if (shipmentMethod != shipment.Method) shipment.Method = shipmentMethod;
            }

            shipment.Cost = shipment.CalculateShippingCost();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ShipmentDTO>.Success(new ShipmentDTO(shipment));
        }
    }
}
