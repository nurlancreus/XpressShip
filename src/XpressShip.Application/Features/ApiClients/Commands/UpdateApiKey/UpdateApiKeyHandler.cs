using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain;
using XpressShip.Domain.Entities;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Responses;
using XpressShip.Application.Abstractions;
using XpressShip.Domain.Abstractions;
using XpressShip.Application.Abstractions.Repositories;

namespace XpressShip.Application.Features.ApiClients.Commands.UpdateApiKey
{
    public class UpdateApiKeyHandler : ICommandHandler<UpdateApiKeyCommand, string>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateApiKeyHandler(IApiClientRepository apiClientRepository, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UpdateApiKeyCommand request, CancellationToken cancellationToken)
        {
            ApiClient? apiClient = await _apiClientRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (apiClient is null) return Result<string>.Failure(Error.NotFoundError(nameof(apiClient)));

            apiClient.UpdateApiKey();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(apiClient.ApiKey);
        }
    }
}
