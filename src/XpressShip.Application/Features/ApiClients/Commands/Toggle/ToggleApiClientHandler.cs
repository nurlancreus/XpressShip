using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.ApiClients.Commands.Toggle
{
    public class ToggleApiClientHandler : ICommandHandler<ToggleApiClientCommand>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ToggleApiClientHandler(IApiClientRepository apiClientRepository, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(ToggleApiClientCommand request, CancellationToken cancellationToken)
        {
            ApiClient? apiClient = await _apiClientRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (apiClient is null) return Result<Unit>.Failure(Error.NotFoundError(nameof(apiClient)));

            apiClient.Toggle();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
