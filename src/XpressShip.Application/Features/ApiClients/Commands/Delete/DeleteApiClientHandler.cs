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

namespace XpressShip.Application.Features.ApiClients.Commands.Delete
{
    public class DeleteApiClientHandler : IRequestHandler<DeleteApiClientCommand, BaseResponse>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteApiClientHandler(IApiClientRepository apiClientRepository, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(DeleteApiClientCommand request, CancellationToken cancellationToken)
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

            _apiClientRepository.Delete(apiClient);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BaseResponse
            {
                IsSuccess = true,
                Message = "Api client deleted successfully"
            };
        }
    }
}
