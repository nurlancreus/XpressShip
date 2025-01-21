﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.ApiClients.DTOs
{
    public record ApiClientDTO
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public AddressDTO? Address { get; set; }  // Navigation Property to Address
        public ICollection<ShipmentDTO> Shipments { get; set; } = []; // Navigation Property to Shipments

        public ApiClientDTO() { }

        public ApiClientDTO(ApiClient apiClient)
        {
            Id = apiClient.Id;
            CompanyName = apiClient.CompanyName;
            ApiKey = apiClient.ApiKey;
            SecretKey = apiClient.SecretKey;
            IsActive = apiClient.IsActive;
            Email = apiClient.Email;
            Country = apiClient.Address.City.Country.Name;
            City = apiClient.Address.City.Name;
            Street = apiClient.Address.Street;
            PostalCode = apiClient.Address.PostalCode;
            CreatedAt = apiClient.CreatedAt;
            Address = new AddressDTO(apiClient.Address);
            Shipments = apiClient.Shipments.Select(s => new ShipmentDTO(s)).ToList();
        }
    }
}
