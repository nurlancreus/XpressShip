using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services;
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
        private readonly IAddressValidationService _addressValidationService;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateApiClientHandler(IApiClientRepository apiClientRepository, ICountryRepository countryRepository, IAddressValidationService addressValidationService, IGeoInfoService geoInfoService, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _countryRepository = countryRepository;
            _addressValidationService = addressValidationService;
            _geoInfoService = geoInfoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ApiClientDTO>> Handle(CreateApiClientCommand request, CancellationToken cancellationToken)
        {
            var isClientByEmailExist = await _apiClientRepository.IsExistAsync(c => c.Email == request.Email, cancellationToken);

            if (isClientByEmailExist) return Result<ApiClientDTO>.Failure(Error.ConflictError($"Clint by email ({request.Email}) is already exists."));

            var apiClient = ApiClient.Create(request.CompanyName, request.Email);

            await _addressValidationService.ValidateCountryCityAndPostalCodeAsync(request.Address.Country, request.Address.City, request.Address.PostalCode, true, cancellationToken);

            var response = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Address.Country, request.Address.City, cancellationToken);

            var lat = response.Latitude;
            var lon = response.Longitude;

            var address = Address.Create(request.Address.PostalCode, request.Address.Street, lat, lon);

            var country = await _countryRepository.Table
                                .Include(c => c.Cities)
                                .Select(c => new { c.Name, c.Cities })
                                .FirstOrDefaultAsync(c => c.Name == request.Address.Country, cancellationToken);

            country = country.EnsureNonNull();

            var city = country.Cities.FirstOrDefault(c => c.Name == request.Address.City);

            city = city.EnsureNonNull();

            address.City = city;

            apiClient.Address = address;

            await _apiClientRepository.AddAsync(apiClient, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ApiClientDTO>.Success(new ApiClientDTO(apiClient));
        }
    }
}
