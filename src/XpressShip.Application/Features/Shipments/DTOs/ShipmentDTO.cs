
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Application.Features.Rates.DTOs;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Shipments.DTOs
{
    public record ShipmentDTO
    {
        public Guid Id { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public ShipmentStatus Status { get; set; }
        public ShipmentRateDTO? Rate { get; set; }
        public DateTime EstimatedDate { get; set; }
        public decimal Cost { get; set; }
        public PaymentDTO? Payment { get; set; }
        public AddressDTO? OriginAddress { get; set; }
        public AddressDTO? DestinationAddress { get; set; }
        public ApiClientDTO? ApiClient { get; set; }
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
            DestinationAddress = shipment.DestinationAddress is not null ? new AddressDTO(shipment.DestinationAddress) : null;
            ApiClient = shipment.ApiClient is not null ? new ApiClientDTO(shipment.ApiClient) : null;
            Payment = shipment.Payment is not null ? new PaymentDTO(shipment.Payment) : null;
            CreatedAt = shipment.CreatedAt;
        }
    }
}
