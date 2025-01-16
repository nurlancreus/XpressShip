using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services;
using XpressShip.Application.Responses;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.ApiClients.Commands.Create
{
    public class CreateApiClientHandler : IRequestHandler<CreateApiClientCommand, ResponseWithData<ApiClientDTO>>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateApiClientHandler(IApiClientRepository apiClientRepository, IGeoInfoService geoInfoService, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _geoInfoService = geoInfoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<ApiClientDTO>> Handle(CreateApiClientCommand request, CancellationToken cancellationToken)
        {
            var apiClient = ApiClient.Create(request.CompanyName);

            var response = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Address.Country, request.Address.City);

            var lat = response.Latitude;
            var lon = response.Longitude;

            IValidationService.ValidateAddress(request.Address.Country, request.Address.City, request.Address.PostalCode, request.Address.Street);

            var address = Address.Create(request.Address.Country, request.Address.City, request.Address.State, request.Address.PostalCode, request.Address.Street, lat, lon);

            apiClient.Address = address;

            await _apiClientRepository.AddAsync(apiClient, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new ApiClientDTO(apiClient);

            return new ResponseWithData<ApiClientDTO>
            {
                IsSuccess = true,
                Message = "Api client created successfully",
                Data = dto
            };
        }
    }
}
