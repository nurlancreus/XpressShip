using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.ApiClients.Commands.Toggle
{
    public class ToggleApiClientHandler : ICommandHandler<ToggleApiClientCommand>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ToggleApiClientHandler(IApiClientSession apiClientSession, IApiClientRepository apiClientRepository, IUnitOfWork unitOfWork)
        {
            _apiClientSession = apiClientSession;
            _apiClientRepository = apiClientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(ToggleApiClientCommand request, CancellationToken cancellationToken)
        {
            ApiClient? apiClient = await _apiClientRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (apiClient is null) return Result<Unit>.Failure(Error.NotFoundError("Client is not found"));

            var keysResult = _apiClientSession.GetClientApiAndSecretKey();

            if (keysResult.IsFailure) return Result<Unit>.Failure(keysResult.Error);

            if (apiClient.SecretKey != keysResult.Value.secretKey || apiClient.ApiKey != keysResult.Value.apiKey) return Result<Unit>.Failure(Error.UnauthorizedError("You are not authorized to toggle the client"));

            apiClient.Toggle();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
