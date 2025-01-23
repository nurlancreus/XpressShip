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

namespace XpressShip.Application.Features.ApiClients.Commands.Create
{
    public record CreateApiClientCommand : ICommand<ApiClientDTO>
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public AddressCommandDTO Address { get; set; } = null!;
    }
}
