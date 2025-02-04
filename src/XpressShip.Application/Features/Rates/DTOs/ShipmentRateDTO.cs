using System;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Rates.DTOs
{
    public record ShipmentRateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BaseRate { get; set; }
        public double MinWeight { get; set; }
        public double MaxWeight { get; set; }
        public double MinDistance { get; set; }
        public double MaxDistance { get; set; }
        public double MinVolume { get; set; }
        public double MaxVolume { get; set; }
        public double BaseRateForKm { get; set; }
        public double BaseRateForKg { get; set; }
        public double BaseRateForVolume { get; set; }
        public double ExpressRateMultiplier { get; set; }
        public double OvernightRateMultiplier { get; set; }
        public double ExpressDeliveryTimeMultiplier { get; set; }
        public double OvernightDeliveryTimeMultiplier { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ShipmentDTO> Shipments { get; set; } = [];

        public ShipmentRateDTO() { }

        public ShipmentRateDTO(ShipmentRate shippingRate)
        {
            Id = shippingRate.Id;
            Name = shippingRate.Name;
            Description = shippingRate.Description;
            BaseRate = shippingRate.BaseRate;
            MinWeight = shippingRate.MinWeight;
            MaxWeight = shippingRate.MaxWeight;
            MinDistance = shippingRate.MinDistance;
            MaxDistance = shippingRate.MaxDistance;
            MinVolume = shippingRate.MinVolume;
            MaxVolume = shippingRate.MaxVolume;
            BaseRateForKm = shippingRate.BaseRateForKm;
            BaseRateForKg = shippingRate.BaseRateForKg;
            BaseRateForVolume = shippingRate.BaseRateForVolume;
            ExpressRateMultiplier = shippingRate.ExpressRateMultiplier;
            OvernightRateMultiplier = shippingRate.OvernightRateMultiplier;
            ExpressDeliveryTimeMultiplier = shippingRate.ExpressDeliveryTimeMultiplier;
            OvernightDeliveryTimeMultiplier = shippingRate.OvernightDeliveryTimeMultiplier;
            CreatedAt = shippingRate.CreatedAt;
            Shipments = shippingRate.Shipments?.Select(s => new ShipmentDTO(s)).ToList() ?? [];
        }
    }
}
