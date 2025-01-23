using MediatR;
using XpressShip.Domain.Entities;
using XpressShip.Domain;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Responses;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Abstractions;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Features.ApiClients.Commands.UpdateSecretKey
{
    public class UpdateSecretKeyHandler : ICommandHandler<UpdateSecretKeyCommand, string>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateSecretKeyHandler(IApiClientRepository apiClientRepository, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UpdateSecretKeyCommand request, CancellationToken cancellationToken)
        {
            ApiClient? apiClient = await _apiClientRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (apiClient is null) return Result<string>.Failure(Error.NotFoundError(nameof(apiClient)));

            apiClient.UpdaterSecretKey();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(apiClient.SecretKey);
        }
    }
}
