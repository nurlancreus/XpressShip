
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Features.Rates.DTOs;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Shipments.DTOs
{
    public record ShipmentDTO
    {
        public Guid Id { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;  // Unique tracking number
        public ShipmentStatus Status { get; set; }  // Using enum for status
        public ShipmentRateDTO? Rate { get; set; }
        public DateTime EstimatedDate { get; set; }
        public decimal Cost { get; set; } // Calculated property to get cost from rate

        public AddressDTO? OriginAddress { get; set; } // Navigation property to Origin Address if null then origin address taken from client (company)
        public AddressDTO? DestinationAddress { get; set; } // Navigation property to Destination Address

        public ApiClientDTO? ApiClient { get; set; } // Navigation property to ApiClient
        public DateTime CreatedAt { get; set; }

        public ShipmentDTO() { }
        public ShipmentDTO(Shipment shipment)
        {
            Id = shipment.Id;
            TrackingNumber = shipment.TrackingNumber;
            Status = shipment.Status;
            Rate = new ShipmentRateDTO(shipment.Rate);
            Cost = shipment.Cost;
            EstimatedDate = shipment.EstimatedDate;
            OriginAddress = shipment.OriginAddress is not null ? new AddressDTO(shipment.OriginAddress) : ApiClient?.Address;
            DestinationAddress = new AddressDTO(shipment.DestinationAddress);
            ApiClient = new ApiClientDTO(shipment.ApiClient);
            CreatedAt = shipment.CreatedAt;
        }
    }
}
