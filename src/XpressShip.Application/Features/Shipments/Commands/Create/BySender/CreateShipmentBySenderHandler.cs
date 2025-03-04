﻿using Microsoft.AspNetCore.Identity;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions.Services.Notification;

namespace XpressShip.Application.Features.Shipments.Commands.Create.BySender
{
    public class CreateShipmentBySenderHandler : ICommandHandler<CreateShipmentBySenderCommand, ShipmentDTO>
    {
        private readonly IJwtSession _jwtSession;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICountryRepository _countryRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentRateRepository _shipmentRateRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAdminNotificationService _adminNotificationService;

        public CreateShipmentBySenderHandler(
            IJwtSession jwtSession,
            UserManager<ApplicationUser> userManager,
            IShipmentRepository shipmentRepository,
            IShipmentRateRepository shipmentRateRepository,
            IGeoInfoService geoInfoService,
            ICountryRepository countryRepository,
            IUnitOfWork unitOfWork,
            IAdminNotificationService adminNotificationService)
        {
            _jwtSession = jwtSession;
            _userManager = userManager;
            _shipmentRepository = shipmentRepository;
            _shipmentRateRepository = shipmentRateRepository;
            _geoInfoService = geoInfoService;
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
            _adminNotificationService = adminNotificationService;
        }

        public async Task<Result<ShipmentDTO>> Handle(CreateShipmentBySenderCommand request, CancellationToken cancellationToken)
        {
            var senderIdResult = _jwtSession.GetUserId();

            if (senderIdResult.IsFailure) return Result<ShipmentDTO>.Failure(senderIdResult.Error);

            // Validate API client
            var sender = await _userManager.Users.OfType<Sender>()
                            .Include(s => s.Address)
                                .ThenInclude(a => a.City)
                                    .ThenInclude(c => c.Country)
                            .FirstOrDefaultAsync(s => s.Id == senderIdResult.Value, cancellationToken);

            if (sender is null)
                return Result<ShipmentDTO>.Failure(Error.NotFoundError("Sender is not found"));

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
                originLat = sender.Address.Latitude;

                originLon = sender.Address.Longitude;

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
            shipment.SenderId = sender.Id;

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

            await _adminNotificationService.SendNewShipmentNotificationAsync(shipment, cancellationToken);

            return Result<ShipmentDTO>.Success(new ShipmentDTO(shipment));
        }
    }
}
