using XpressShip.Application.Abstractions;
using XpressShip.Domain.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;

namespace XpressShip.Application.Features.ApiClients.Commands.UpdateApiKey
{
    public class UpdateApiKeyHandler : ICommandHandler<UpdateApiKeyCommand, string>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateApiKeyHandler(IApiClientSession apiClientSession, IApiClientRepository apiClientRepository, IUnitOfWork unitOfWork)
        {
            _apiClientSession = apiClientSession;
            _apiClientRepository = apiClientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UpdateApiKeyCommand request, CancellationToken cancellationToken)
        {
            var apiClient = await _apiClientRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (apiClient is null) return Result<string>.Failure(Error.NotFoundError("Client is not found"));

            var clientIdResult = _apiClientSession.GetClientId();

            if (clientIdResult.IsFailure) return Result<string>.Failure(clientIdResult.Error);

            if (apiClient.Id != clientIdResult.Value) return Result<string>.Failure(Error.UnauthorizedError("You are not authorized to update the api key"));

            var newApiKey = apiClient.UpdateApiKey();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(newApiKey);
        }
    }
}
