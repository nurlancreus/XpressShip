using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Extensions;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.ApiClients.Commands.Create
{
    public class CreateApiClientHandler : ICommandHandler<CreateApiClientCommand, ApiClientDTO>
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

        public async Task<Result<ApiClientDTO>> Handle(CreateApiClientCommand request, CancellationToken cancellationToken)
        {
            var apiClient = ApiClient.Create(request.CompanyName, request.Email);

            var geoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Address.Country, request.Address.City, cancellationToken);

            if (!geoInfoResult.IsSuccess) return Result<ApiClientDTO>.Failure(geoInfoResult.Error);

            var lat = geoInfoResult.Value.Latitude;
            var lon = geoInfoResult.Value.Longitude;

            var address = Address.Create(request.Address.PostalCode, request.Address.Street, lat, lon);

            var country = await _countryRepository.Table
                                .Include(c => c.Cities)
                                .Select(c => new { c.Name, c.Cities })
                                .FirstOrDefaultAsync(c => c.Name == request.Address.Country, cancellationToken);

            country = country.EnsureNonNull();

            if (country is null) return Result<ApiClientDTO>.Failure(Error.BadRequestError("Country is not supported"));

            var city = country.Cities.FirstOrDefault(c => c.Name == request.Address.City);

            if (city is null) return Result<ApiClientDTO>.Failure(Error.BadRequestError("City is not supported"));

            address.City = city;

            apiClient.Address = address;

            await _apiClientRepository.AddAsync(apiClient, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ApiClientDTO>.Success(new ApiClientDTO(apiClient));
        }
    }
}
