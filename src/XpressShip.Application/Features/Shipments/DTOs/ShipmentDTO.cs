using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Application.Features.Rates.DTOs;
using XpressShip.Application.Features.User.DTOs;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.Shipments.DTOs
{
    public record ShipmentDTO
    {
        public Guid Id { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public ShipmentRateDTO? Rate { get; set; }
        public DateTime? EstimatedDate { get; set; }
        public decimal Cost { get; set; }
        public PaymentDTO? Payment { get; set; }
        public AddressDTO? OriginAddress { get; set; }
        public AddressDTO? DestinationAddress { get; set; }
        public ApiClientDTO? ApiClient { get; set; }
        public SenderDTO? Sender { get; set; }
        public DateTime CreatedAt { get; set; }

        public ShipmentDTO() { }
        public ShipmentDTO(Shipment shipment)
        {
            Id = shipment.Id;
            TrackingNumber = shipment.TrackingNumber;
            Status = shipment.Status.ToString();
            Rate = shipment.Rate is not null ? new ShipmentRateDTO(shipment.Rate) : null;
            Cost = shipment.Cost;
            EstimatedDate = shipment.EstimatedDate;
            OriginAddress = shipment.OriginAddress is not null ? new AddressDTO(shipment.OriginAddress) : ApiClient?.Address ?? Sender?.Address;
            DestinationAddress = shipment.DestinationAddress is not null ? new AddressDTO(shipment.DestinationAddress) : null;
            ApiClient = shipment.ApiClient is not null ? new ApiClientDTO(shipment.ApiClient) : null;
            Sender = shipment.Sender is not null ? new SenderDTO(shipment.Sender) : null;
            Payment = shipment.Payment is not null ? new PaymentDTO(shipment.Payment) : null;
            CreatedAt = shipment.CreatedAt;
        }
    }
}
