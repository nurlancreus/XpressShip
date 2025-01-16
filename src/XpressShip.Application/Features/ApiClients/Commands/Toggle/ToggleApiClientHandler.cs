using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Responses;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.ApiClients.Commands.Toggle
{
    public class ToggleApiClientHandler : IRequestHandler<ToggleApiClientCommand, BaseResponse>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ToggleApiClientHandler(IApiClientRepository apiClientRepository, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(ToggleApiClientCommand request, CancellationToken cancellationToken)
        {
            ApiClient? apiClient = await _apiClientRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (apiClient is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Api client not found"
                };
            }

            if (!apiClient.IsActive) apiClient.IsActive = true;
            else apiClient.IsActive = false;


            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BaseResponse
            {
                IsSuccess = true,
                Message = "Api client updated successfully"
            };
        }
    }
}
