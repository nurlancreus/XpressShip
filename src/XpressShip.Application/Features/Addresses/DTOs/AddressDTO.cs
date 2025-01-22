
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
        public ICollection<ShipmentDTO?> ShipmentsOrigin { get; set; } = [];
        public ICollection<ShipmentDTO?> ShipmentsDestination { get; set; } = [];
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
            ShipmentsOrigin = address.ShipmentsOrigin.Select(s => s != null ? new ShipmentDTO(s) : null).ToList();
            ShipmentsDestination = address.ShipmentsDestination.Select(s => s != null ? new ShipmentDTO(s) : null).ToList();
        }
    }
}
