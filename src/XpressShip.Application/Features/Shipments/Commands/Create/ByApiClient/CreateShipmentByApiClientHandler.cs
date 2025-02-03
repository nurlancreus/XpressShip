using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;
using Microsoft.EntityFrameworkCore;

namespace XpressShip.Application.Features.Shipments.Commands.Create.ByApiClient
{
    public class CreateShipmentByApiClientHandler : ICommandHandler<CreateShipmentByApiClientCommand, ShipmentDTO>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly ICountryRepository _countryRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentRateRepository _shipmentRateRepository;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateShipmentByApiClientHandler(
            IApiClientSession apiClientSession,
            IShipmentRepository shipmentRepository,
            IShipmentRateRepository shipmentRateRepository,
            IApiClientRepository apiClientRepository,
            IGeoInfoService geoInfoService,
            ICountryRepository countryRepository,
            IUnitOfWork unitOfWork)
        {
            _apiClientSession = apiClientSession;
            _shipmentRepository = shipmentRepository;
            _shipmentRateRepository = shipmentRateRepository;
            _apiClientRepository = apiClientRepository;
            _geoInfoService = geoInfoService;
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ShipmentDTO>> Handle(CreateShipmentByApiClientCommand request, CancellationToken cancellationToken)
        {
            var keysResult = _apiClientSession.GetClientApiAndSecretKey();

            if (keysResult.IsFailure) return Result<ShipmentDTO>.Failure(keysResult.Error);

            // Validate API client
            var client = await _apiClientRepository.Table
                .Include(c => c.Address)
                    .ThenInclude(a => a.City)
                        .ThenInclude(c => c.Country)
                .FirstOrDefaultAsync(a => a.ApiKey == keysResult.Value.apiKey && a.SecretKey == keysResult.Value.secretKey && a.IsActive, cancellationToken);

            if (client is null)
                return Result<ShipmentDTO>.Failure(Error.NotFoundError("Client is not found"));

            // Validate shipment rate
            var shipmentRate = await _shipmentRateRepository.GetByIdAsync(request.ShipmentRateId, true, cancellationToken);

            if (shipmentRate is null)
                return Result<ShipmentDTO>.Failure(Error.NotFoundError("Rate is not found"));

            double? originLat = null;
            double? originLon = null;

            Address? originAddress = null;

            if (request.Origin is not null)
            {
                var originGeoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Origin.Country, request.Origin.City, cancellationToken);

                if (originGeoInfoResult.IsFailure) return Result<ShipmentDTO>.Failure(originGeoInfoResult.Error);

                originLat = originGeoInfoResult.Value.Latitude;
                originLon = originGeoInfoResult.Value.Longitude;

                originAddress = Address.Create(request.Origin.PostalCode, request.Origin.Street, (double)originLat!, (double)originLon!);

                var originCountry = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == request.Origin.Country, cancellationToken);

                if (originCountry is null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("Country is not supported"));

                var originCity = originCountry.Cities.FirstOrDefault(c => c.Name == request.Origin.City);

                if (originCity is null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("City is not supported"));

                originAddress.City = originCity;
            }
            else
            {
                originLat = client.Address.Latitude;

                originLon = client.Address.Longitude;

                if (originLat == null || originLon == null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("Origin address's latitude and longitude cannot be null"));
            }

            // Calculate and validate distance
            var destinationGeoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Destination.Country, request.Destination.City, cancellationToken);

            if (destinationGeoInfoResult.IsFailure) return Result<ShipmentDTO>.Failure(destinationGeoInfoResult.Error);

            // Validate method
            if (!Enum.TryParse<ShipmentMethod>(request.Method, true, out var method)) return Result<ShipmentDTO>.Failure(Error.BadRequestError("Could not parse the enum"));

            // Create shipment
            var shipment = Shipment.Create(request.Weight, request.Dimensions, method, request.Note);

            shipment.Rate = shipmentRate;

            shipment.ApiClientId = client.Id;

            // Create and validate adresses
            var destinationAddress = Address.Create(request.Destination.PostalCode, request.Destination.Street, destinationGeoInfoResult.Value.Latitude, destinationGeoInfoResult.Value.Longitude);

            var country = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == request.Destination.Country, cancellationToken);

            if (country is null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("Country is not supported"));

            var city = country.Cities.FirstOrDefault(c => c.Name == request.Destination.City);

            if (city is null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("City is not supported"));

            destinationAddress.City = city;

            shipment.DestinationAddress = destinationAddress;

            if (originAddress is not null) shipment.OriginAddress = originAddress;

            // Calculate final cost
            shipment.Cost = shipment.CalculateShippingCost();

            await _shipmentRepository.AddAsync(shipment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ShipmentDTO>.Success(new ShipmentDTO(shipment));
        }
    }
}
