using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Extensions;

namespace XpressShip.Application.Features.ApiClients.Commands.Create
{
    public class CreateApiClientHandler : ICommandHandler<CreateApiClientCommand, KeysDTO>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateApiClientHandler(IApiClientRepository apiClientRepository, ICountryRepository countryRepository, IGeoInfoService geoInfoService, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _countryRepository = countryRepository;
            _geoInfoService = geoInfoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<KeysDTO>> Handle(CreateApiClientCommand request, CancellationToken cancellationToken)
        {
            var (client, rawSecretKey) = ApiClient.Create(request.CompanyName, request.Email);

            var geoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Address.Country, request.Address.City, cancellationToken);

            if (!geoInfoResult.IsSuccess) return Result<KeysDTO>.Failure(geoInfoResult.Error);

            var lat = geoInfoResult.Value.Latitude;
            var lon = geoInfoResult.Value.Longitude;

            var address = Address.Create(request.Address.PostalCode, request.Address.Street, lat, lon);

            var country = await _countryRepository.Table
                                .Include(c => c.Cities)
                                .Select(c => new { c.Name, c.Cities })
                                .FirstOrDefaultAsync(c => c.Name == request.Address.Country, cancellationToken);

            country = country.EnsureNonNull();

            if (country is null) return Result<KeysDTO>.Failure(Error.BadRequestError("Country is not supported"));

            var city = country.Cities.FirstOrDefault(c => c.Name == request.Address.City);

            if (city is null) return Result<KeysDTO>.Failure(Error.BadRequestError("City is not supported"));

            address.City = city;

            client.Address = address;

            await _apiClientRepository.AddAsync(client, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<KeysDTO>.Success(new KeysDTO(rawSecretKey, client.ApiKey));
        }
    }
}
