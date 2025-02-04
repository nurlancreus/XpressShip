using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Features.ApiClients.Queries.GetAll
{
    public class GetAllApiClientsHandler : IQueryHandler<GetAllApiClientsQuery, List<ApiClientDTO>>
    {
        private readonly IApiClientRepository _apiClientRepository;
        private readonly IJwtSession _jwtSession;

        public GetAllApiClientsHandler(IApiClientRepository apiClientRepository, IJwtSession jwtSession)
        {
            _apiClientRepository = apiClientRepository;
            _jwtSession = jwtSession;
        }

        public async Task<Result<List<ApiClientDTO>>> Handle(GetAllApiClientsQuery request, CancellationToken cancellationToken)
        {
            var isAdminResult = _jwtSession.IsAdminAuth();

            if (isAdminResult.IsFailure) return Result<List<ApiClientDTO>>.Failure(isAdminResult.Error);

            var clients = _apiClientRepository.Table
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
                                .AsNoTracking();

            var dtos = await clients.Select(client => new ApiClientDTO(client)).ToListAsync(cancellationToken);

            return Result<List<ApiClientDTO>>.Success(dtos);
        }
    }
}
