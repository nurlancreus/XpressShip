using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.ApiClients.Queries.Get
{
    public class GetApiClientByIdHandler : IQueryHandler<GetApiClientByIdQuery, ApiClientDTO>
    {
        private readonly IApiClientSessionService _apiClientSessionService;
        private readonly IApiClientRepository _apiClientRepository;
        private readonly bool IsAdmin = true;
        public GetApiClientByIdHandler(IApiClientSessionService apiClientSessionService, IApiClientRepository apiClientRepository)
        {
            _apiClientSessionService = apiClientSessionService;
            _apiClientRepository = apiClientRepository;
        }

        public async Task<Result<ApiClientDTO>> Handle(GetApiClientByIdQuery request, CancellationToken cancellationToken)
        {
            ApiClient? apiClient = await _apiClientRepository.Table
                                .Include(c => c.Address)
                                .Include(c => c.Shipments)
                                    .ThenInclude(s => s.Rate)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (apiClient is null)
            {
                return Result<ApiClientDTO>.Failure(Error.NotFoundError(nameof(apiClient)));
            }

            var keys = _apiClientSessionService.GetClientApiAndSecretKey();

            if (keys is (string apikey, string secretKey))
            {
                if (apiClient.ApiKey != apikey || apiClient.SecretKey != secretKey)
                    return Result<ApiClientDTO>.Failure(Error.UnauthorizedError("You're not authorized to get payment details!"));

            }
            else if (!IsAdmin) return Result<ApiClientDTO>.Failure(Error.UnauthorizedError("You're not authorized to get payment details!"));

            return Result<ApiClientDTO>.Success(new ApiClientDTO(apiClient));
        }
    }
}
