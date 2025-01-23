using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Shipments.Commands.Delete
{
    public record DeleteShipmentCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}
