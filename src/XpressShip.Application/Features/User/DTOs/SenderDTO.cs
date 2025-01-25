using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.User.DTOs
{
    public record SenderDTO : UserDTO
    {
        public AddressDTO? Address { get; set; } = null!;
        public IEnumerable<ShipmentDTO> Shipments { get; set; } = [];

        public SenderDTO(Sender sender) : base(sender)
        {
            Address = new AddressDTO(sender.Address);
            Shipments = sender.Shipments.Count > 0 ? sender.Shipments.Select(s => new ShipmentDTO(s)) : [];
        }
    }
}
