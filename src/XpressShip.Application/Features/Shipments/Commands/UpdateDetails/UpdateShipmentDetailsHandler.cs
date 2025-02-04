using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Abstractions;
using XpressShip.Domain.Abstractions;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateDetails
{
    public class UpdateShipmentDetailsHandler : ICommandHandler<UpdateShipmentDetailsCommand, ShipmentDTO>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IJwtSession _jwtSession;
        private readonly ICountryRepository _countryRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentRateRepository _shipmentRateRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateShipmentDetailsHandler(IApiClientSession apiClientSession, IJwtSession jwtSession, ICountryRepository countryRepository, IShipmentRepository shipmentRepository, IShipmentRateRepository shipmentRateRepository, IGeoInfoService geoInfoService, IUnitOfWork unitOfWork)
        {
            _apiClientSession = apiClientSession;
            _jwtSession = jwtSession;
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
                                    .ThenInclude(a => a!.City)
                                         .ThenInclude(c => c.Country)
                                .Include(s => s.DestinationAddress)
                                    .ThenInclude(a => a.City)
                                         .ThenInclude(c => c.Country)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                                        .ThenInclude(a => a.City)
                                            .ThenInclude(c => c.Country)
                                .Include(s => s.Sender)
                                    .ThenInclude(s => s!.Address)
                                        .ThenInclude(a => a.City)
                                            .ThenInclude(c => c.Country)
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment is null) return Result<ShipmentDTO>.Failure(Error.NotFoundError("Shipment is not found"));

            if (shipment.ApiClient is ApiClient apiClient)
            {
                var clientIdResult = _apiClientSession.GetClientId();

                if (clientIdResult.IsFailure) return Result<ShipmentDTO>.Failure(clientIdResult.Error);

                if (apiClient.Id != clientIdResult.Value)
                {
                    return Result<ShipmentDTO>.Failure(Error.UnauthorizedError("You cannot update this shipment"));
                }
            }
            else if (shipment.Sender is Sender sender)
            {
                var senderIdResult = _jwtSession.GetUserId();

                if (senderIdResult.IsFailure) return Result<ShipmentDTO>.Failure(senderIdResult.Error);

                if (sender.Id != senderIdResult.Value)
                {
                    return Result<ShipmentDTO>.Failure(Error.UnauthorizedError("You cannot update this shipment"));
                }
            }
            else return Result<ShipmentDTO>.Failure(Error.UnexpectedError("Shipment initiator is not found"));

            // Validate shipment rate
            var shipmentRate = shipment.Rate;

            if (request.ShipmentRateId is Guid rateId && shipmentRate.Id != rateId)
            {
                shipmentRate = await _shipmentRateRepository.GetByIdAsync(rateId, true, cancellationToken);

                if (shipmentRate is null) return Result<ShipmentDTO>.Failure(Error.NotFoundError("Rate is not found"));

                shipment.Rate = shipmentRate;
            }

            if (request.Origin is AddressCommandDTO originAddress)
            {
                var originGeoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(originAddress.Country, originAddress.City, cancellationToken);

                if (originGeoInfoResult.IsFailure) return Result<ShipmentDTO>.Failure(originGeoInfoResult.Error);

                shipment.OriginAddress = Address.Create(originAddress.PostalCode, originAddress.Street, originGeoInfoResult.Value.Latitude, originGeoInfoResult.Value.Longitude);

                var country = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == originAddress.Country, cancellationToken);

                if (country is null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("Country is not supported"));

                var city = country.Cities.FirstOrDefault(c => c.Name == originAddress.City);

                if (city is null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("City is not supported"));

                shipment.OriginAddress.City = city;
            }

            if (request.Destination is AddressCommandDTO destinationAddress)
            {
                var destinationGeoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(destinationAddress.Country, destinationAddress.City, cancellationToken);

                if (destinationGeoInfoResult.IsFailure) return Result<ShipmentDTO>.Failure(destinationGeoInfoResult.Error);

                shipment.DestinationAddress = Address.Create(destinationAddress.PostalCode, destinationAddress.Street, destinationGeoInfoResult.Value.Latitude, destinationGeoInfoResult.Value.Longitude);

                var country = await _countryRepository.Table
                            .Include(c => c.Cities)
                            .Select(c => new { c.Name, c.Cities })
                            .FirstOrDefaultAsync(c => c.Name == destinationAddress.Country, cancellationToken);

                if (country is null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("Country is not supported"));

                var city = country.Cities.FirstOrDefault(c => c.Name == destinationAddress.City);

                if (city is null) return Result<ShipmentDTO>.Failure(Error.BadRequestError("City is not supported"));

                shipment.DestinationAddress.City = city;
            }

            shipment.Update(request.Weight, request.Dimensions, request.Method, request.Note);

            shipment.Cost = shipment.CalculateShippingCost();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ShipmentDTO>.Success(new ShipmentDTO(shipment));
        }
    }
}
