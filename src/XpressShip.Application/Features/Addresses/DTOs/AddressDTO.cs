
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.Addresses.DTOs
{
    public record AddressDTO
    {
        public Guid Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApiClientDTO? Client { get; set; }
        public IEnumerable<ShipmentDTO> ShipmentsOrigin { get; set; } = [];
        public IEnumerable<ShipmentDTO> ShipmentsDestination { get; set; } = [];
        public AddressDTO() { }
        public AddressDTO(Address address)
        {
            Id = address.Id;
            Street = address.Street;
            City = address.City.Name;
            PostalCode = address.PostalCode;
            Country = address.City.Country.Name;
            Latitude = address.Latitude;
            Longitude = address.Longitude;
            CreatedAt = address.CreatedAt;
            Client = address.Client is not null ? new ApiClientDTO(address.Client) : null;
            ShipmentsOrigin = address.ShipmentsOrigin.Count > 0 ? address.ShipmentsOrigin.Select(s => new ShipmentDTO(s)) : [];
            ShipmentsDestination = address.ShipmentsDestination.Count > 0 ? address.ShipmentsDestination.Select(s => new ShipmentDTO(s)) : [];
        }
    }
}
