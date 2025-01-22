using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.ApiClients.Queries.GetAll
{
    public class GetAllApiClientsHandler : IRequestHandler<GetAllApiClientsQuery, ResponseWithData<IEnumerable<ApiClientDTO>>>
    {
        private readonly IApiClientRepository _apiClientRepository;

        public GetAllApiClientsHandler(IApiClientRepository apiClientRepository)
        {
            _apiClientRepository = apiClientRepository;
        }

        public async Task<ResponseWithData<IEnumerable<ApiClientDTO>>> Handle(GetAllApiClientsQuery request, CancellationToken cancellationToken)
        {
            var clients = _apiClientRepository.Table
                                .Include(c => c.Address)
                                .Include(c => c.Shipments)
                                    .ThenInclude(s => s.Rate)
                                .AsNoTracking();

            return new ResponseWithData<IEnumerable<ApiClientDTO>>
            {
                IsSuccess = true,
                Data = await clients.Select(client => new ApiClientDTO(client)).ToListAsync(cancellationToken),
                Message = "Api clients retrieved successfully",
            };
        }
    }
}
