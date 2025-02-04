using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.ApiClients.Queries.Get
{
    public class GetApiClientByIdHandler : IQueryHandler<GetApiClientByIdQuery, ApiClientDTO>
    {
        private readonly IApiClientSession _apiClientSessionService;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IJwtSession _jwtSession;
        public GetApiClientByIdHandler(IApiClientSession apiClientSessionService, IApiClientRepository apiClientRepository, IJwtSession jwtSession)
        {
            _apiClientSessionService = apiClientSessionService;
            _apiClientRepository = apiClientRepository;
            _jwtSession = jwtSession;
        }

        public async Task<Result<ApiClientDTO>> Handle(GetApiClientByIdQuery request, CancellationToken cancellationToken)
        {
            ApiClient? apiClient = await _apiClientRepository.Table
                                .Include(c => c.Address)
                                    .ThenInclude(a => a.City)
                                        .ThenInclude(c => c.Country)
                                .Include(c => c.Shipments)
                                    .ThenInclude(s => s.Rate)
                                .Include(c => c.Shipments)
                                    .ThenInclude(s => s.DestinationAddress)
                                        .ThenInclude(a => a.City)
                                            .ThenInclude(c => c.Country)
                                .Include(c => c.Shipments)
                                    .ThenInclude(s => s.OriginAddress)
                                        .ThenInclude(a => a!.City)
                                            .ThenInclude(c => c.Country)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (apiClient is null)
                return Result<ApiClientDTO>.Failure(Error.NotFoundError("Client is not found"));

            var isAdminResult = _jwtSession.IsAdminAuth();

            if (isAdminResult.IsFailure)
            {
                var clientIdResult = _apiClientSessionService.GetClientId();

                if (clientIdResult.IsFailure) return Result<ApiClientDTO>.Failure(clientIdResult.Error);

                if (apiClient.Id != clientIdResult.Value)
                    return Result<ApiClientDTO>.Failure(Error.UnauthorizedError("You're not authorized to get client details!"));
            }

            return Result<ApiClientDTO>.Success(new ApiClientDTO(apiClient));
        }
    }
}
