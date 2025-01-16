using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.ApiClients.Commands.Toggle
{
    public record ToggleApiClientCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
    }
}
