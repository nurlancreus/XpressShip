using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.ApiClients.DTOs;

namespace XpressShip.Application.Features.ApiClients.Queries.Get
{
    public record GetApiClientByIdQuery : IQuery<ApiClientDTO>
    {
        public Guid Id { get; set; }
    }
}
