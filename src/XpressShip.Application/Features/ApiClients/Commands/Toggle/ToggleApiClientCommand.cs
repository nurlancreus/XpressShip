using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.ApiClients.Commands.Toggle
{
    public record ToggleApiClientCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}
