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

namespace XpressShip.Application.Features.ApiClients.Commands.Delete
{
    public class DeleteApiClientHandler : ICommandHandler<DeleteApiClientCommand, Guid>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteApiClientHandler(IApiClientRepository apiClientRepository, IUnitOfWork unitOfWork)
        {
            _apiClientRepository = apiClientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteApiClientCommand request, CancellationToken cancellationToken)
        {
            ApiClient? apiClient = await _apiClientRepository.GetByIdAsync(request.Id, true, cancellationToken);

            if (apiClient is null) return Result<Guid>.Failure(Error.NotFoundError(nameof(apiClient)));


            _apiClientRepository.Delete(apiClient);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(apiClient.Id);
        }
    }
}
