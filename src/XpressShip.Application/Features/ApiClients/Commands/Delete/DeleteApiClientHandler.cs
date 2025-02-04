using MediatR;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Features.ApiClients.Commands.Delete
{
    public class DeleteApiClientHandler : ICommandHandler<DeleteApiClientCommand, Unit>
    {
        private readonly IJwtSession _jwtSession;
        private readonly IApiClientSession _apiClientSession;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteApiClientHandler(IJwtSession jwtSession, IApiClientSession apiClientSession, IApiClientRepository apiClientRepository, IUnitOfWork unitOfWork)
        {
            _jwtSession = jwtSession;
            _apiClientSession = apiClientSession;
            _apiClientRepository = apiClientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(DeleteApiClientCommand request, CancellationToken cancellationToken)
        {
            var apiClient = await _apiClientRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (apiClient is null) return Result<Unit>.Failure(Error.NotFoundError("Client is not found"));

            var isAdminResult = _jwtSession.IsAdminAuth();

            if (isAdminResult.IsFailure)
            {
                var clientIdResult = _apiClientSession.GetClientId();

                if (clientIdResult.IsFailure) return Result<Unit>.Failure(clientIdResult.Error);

                if (apiClient.Id != clientIdResult.Value) return Result<Unit>.Failure(Error.UnauthorizedError("You are not authorized to delete the client"));
            }

            _apiClientRepository.Delete(apiClient);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
