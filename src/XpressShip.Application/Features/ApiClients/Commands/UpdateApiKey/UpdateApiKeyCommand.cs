using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.ApiClients.Commands.UpdateApiKey
{
    public record UpdateApiKeyCommand : ICommand<string>
    {
        public Guid Id { get; set; }
    }
}
