using MediatR;
using XpressShip.Domain.Entities;
using XpressShip.Domain;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Responses;
using XpressShip.Application.Interfaces;

namespace XpressShip.Application.Features.ApiClients.Commands.UpdateSecretKey
{
    public class UpdateSecretKeyHandler : IRequestHandler<UpdateSecretKeyCommand, ResponseWithData<ApiClientDTO>>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateSecretKeyHandler(IApiClientRepository apiClientRepository, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWithData<ApiClientDTO>> Handle(UpdateSecretKeyCommand request, CancellationToken cancellationToken)
        {
            ApiClient? apiClient = await _apiClientRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (apiClient is null)
            {
                return new ResponseWithData<ApiClientDTO>
                {
                    IsSuccess = false,
                    Message = "Api client not found"
                };
            }

            apiClient.SecretKey = IGenerator.GenerateSecretKey();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResponseWithData<ApiClientDTO>
            {
                IsSuccess = true,
                Message = "Secret key updated successfully",
                Data = new ApiClientDTO(apiClient)
            };
        }
    }
}
