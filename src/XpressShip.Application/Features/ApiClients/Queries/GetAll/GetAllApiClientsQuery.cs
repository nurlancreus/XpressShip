using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.ApiClients.DTOs;

namespace XpressShip.Application.Features.ApiClients.Queries.GetAll
{
    public record GetAllApiClientsQuery : IQuery<List<ApiClientDTO>>
    {
    }

}
