using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.ApiClients.Commands.Update
{
    public record UpdateApiClientCommand : ICommand<ApiClientDTO>
    {
        public Guid? Id { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public AddressCommandDTO? Address { get; set; }
    }
}
