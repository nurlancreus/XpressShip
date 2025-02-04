using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.ApiClients.Commands.Update
{
    public class UpdateApiClientHandler : ICommandHandler<UpdateApiClientCommand, Guid>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateApiClientHandler(IApiClientSession apiClientSession, IApiClientRepository apiClientRepository, ICountryRepository countryRepository, IGeoInfoService geoInfoService, IUnitOfWork unitOfWork)
        {
            _apiClientSession = apiClientSession;
            _apiClientRepository = apiClientRepository;
            _countryRepository = countryRepository;
            _geoInfoService = geoInfoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UpdateApiClientCommand request, CancellationToken cancellationToken)
        {
            var apiClient = await _apiClientRepository.Table
                                            .Include(client => client.Address)
                                            .FirstOrDefaultAsync(client => client.Id == request.Id, cancellationToken);

            if (apiClient is null) return Result<Guid>.Failure(Error.NotFoundError("Client is not found"));

            var clientIdResult = _apiClientSession.GetClientId();

            if (clientIdResult.IsFailure) return Result<Guid>.Failure(clientIdResult.Error);

            if (apiClient.Id != clientIdResult.Value) return Result<Guid>.Failure(Error.UnauthorizedError("You are not authorized to update the client"));

            if (request.CompanyName is string companyName && companyName != apiClient.CompanyName)
            {
                apiClient.CompanyName = companyName;
            }

            if (request.Email is string email && email != apiClient.Email)
            {
                var isClientByEmailExist = await _apiClientRepository.IsExistAsync(c => c.Email == email, cancellationToken);

                if (isClientByEmailExist) return Result<Guid>.Failure(Error.ConflictError($"Clint by email ({request.Email}) is already exists."));

                apiClient.Email = email;
            }

            if (request.Address is not null)
            {
                var geoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Address.Country, request.Address.City, cancellationToken);

                if (geoInfoResult.IsFailure) return Result<Guid>.Failure(geoInfoResult.Error);

                var lat = geoInfoResult.Value.Latitude;
                var lon = geoInfoResult.Value.Longitude;

                var address = Address.Create(request.Address.PostalCode, request.Address.Street, lat, lon);

                var country = await _countryRepository.Table
                                .Include(c => c.Cities)
                                .Select(c => new { c.Name, c.Cities })
                                .FirstOrDefaultAsync(c => c.Name == request.Address.Country, cancellationToken);

                if (country is null) return Result<Guid>.Failure(Error.BadRequestError("Country is not supported"));

                var city = country.Cities.FirstOrDefault(c => c.Name == request.Address.City);

                if (city is null) return Result<Guid>.Failure(Error.BadRequestError("City is not supported"));

                address.City = city;

                apiClient.Address = address;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(apiClient.Id);
        }
    }
}
