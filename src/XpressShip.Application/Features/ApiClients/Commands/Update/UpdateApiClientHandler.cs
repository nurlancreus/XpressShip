using MediatR;
using Microsoft.EntityFrameworkCore;
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

namespace XpressShip.Application.Features.ApiClients.Commands.Update
{
    public class UpdateApiClientHandler : IRequestHandler<UpdateApiClientCommand, ResponseWithData<ApiClientDTO>>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateApiClientHandler(IApiClientRepository apiClientRepository, IGeoInfoService geoInfoService, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _geoInfoService = geoInfoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<ApiClientDTO>> Handle(UpdateApiClientCommand request, CancellationToken cancellationToken)
        {
            ApiClient? apiClient = await _apiClientRepository.Table
                                           .Include(client => client.Address)
                                           .FirstOrDefaultAsync(client => client.Id == request.Id, cancellationToken);

            if (apiClient is null)
            {
                return new ResponseWithData<ApiClientDTO>
                {
                    IsSuccess = false,
                    Message = "Api client not found",
                };
            }

            if (request.CompanyName is string companyName && companyName != apiClient.CompanyName)
            {
                apiClient.CompanyName = companyName;
            }

            if (request.Address is not null)
            {
                var response = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Address.Country, request.Address.City);

                var lat = response.Latitude;
                var lon = response.Longitude;

                IValidationService.ValidateAddress(request.Address.Country, request.Address.City, request.Address.PostalCode, request.Address.Street);

                var address = Address.Create(request.Address.Country, request.Address.City, request.Address.State, request.Address.PostalCode, request.Address.Street, lat, lon);

                apiClient.Address = address;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResponseWithData<ApiClientDTO>
            {
                IsSuccess = true,
                Message = "Api client updated successfully",
                Data = new ApiClientDTO(apiClient)
            };
        }
    }
}
