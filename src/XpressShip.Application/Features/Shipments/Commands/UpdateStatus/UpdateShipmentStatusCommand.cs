using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateStatus
{
    public record UpdateShipmentStatusCommand : IRequest<ResponseWithData<ShipmentDTO>>
    {
        public Guid? Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
