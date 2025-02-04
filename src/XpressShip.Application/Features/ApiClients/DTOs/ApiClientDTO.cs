using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.ApiClients.DTOs
{
    public record ApiClientDTO
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public AddressDTO? Address { get; set; }
        public IEnumerable<ShipmentDTO> Shipments { get; set; } = []; 
        public ApiClientDTO() { }

        public ApiClientDTO(ApiClient apiClient)
        {
            Id = apiClient.Id;
            CompanyName = apiClient.CompanyName;
            IsActive = apiClient.IsActive;
            Email = apiClient.Email;
            Country = apiClient.Address.City.Country.Name;
            City = apiClient.Address.City.Name;
            Street = apiClient.Address.Street;
            PostalCode = apiClient.Address.PostalCode;
            CreatedAt = apiClient.CreatedAt;
            Address = new AddressDTO(apiClient.Address);
            Shipments = apiClient.Shipments.Count > 0 ? apiClient.Shipments.Select(s =>  new ShipmentDTO(s)) : [];
        }
    }
}
