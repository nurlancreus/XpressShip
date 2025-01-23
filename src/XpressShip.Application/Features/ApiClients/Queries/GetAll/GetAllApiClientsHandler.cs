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
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Features.ApiClients.Queries.GetAll
{
    public class GetAllApiClientsHandler : IQueryHandler<GetAllApiClientsQuery, List<ApiClientDTO>>
    {
        private readonly IApiClientRepository _apiClientRepository;

        public GetAllApiClientsHandler(IApiClientRepository apiClientRepository)
        {
            _apiClientRepository = apiClientRepository;
        }

        public async Task<Result<List<ApiClientDTO>>> Handle(GetAllApiClientsQuery request, CancellationToken cancellationToken)
        {
            var clients = _apiClientRepository.Table
                                .Include(c => c.Address)
                                .Include(c => c.Shipments)
                                    .ThenInclude(s => s.Rate)
                                .AsNoTracking();

            var dtos = await clients.Select(client => new ApiClientDTO(client)).ToListAsync(cancellationToken);

            return Result<List<ApiClientDTO>>.Success(dtos);
        }
    }
}
