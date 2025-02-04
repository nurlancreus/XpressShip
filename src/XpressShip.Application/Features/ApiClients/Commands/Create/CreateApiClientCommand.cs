using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.ApiClients.DTOs;

namespace XpressShip.Application.Features.ApiClients.Commands.Create
{
    public record CreateApiClientCommand : ICommand<KeysDTO>
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public AddressCommandDTO Address { get; set; } = null!;
    }
}
