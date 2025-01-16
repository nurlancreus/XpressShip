using XpressShip.Domain.Entities.Base;
using XpressShip.Domain.Enums;

namespace XpressShip.Domain.Entities
{
    public class ShipmentRate : BaseEntity
    {
        public string Name { get; set; } = string.Empty;  // E.g., "Small Package - Short Distance"
        public string? Description { get; set; }  // Optional detailed description
        public decimal BaseRate { get; set; }  // Shipping cost based on weight, dimensions, etc.
        public double MinWeight { get; set; }  // Minimum weight for the rate
        public double MaxWeight { get; set; }  // Maximum weight for the rate
        public double MinDistance { get; set; }  // Minimum distance for the rate
        public double MaxDistance { get; set; }  // Maximum distance for the rate
        public double MinVolume { get; set; }  // Minimum volume for the rate
        public double MaxVolume { get; set; }  // Maximum volume for the rate
        public double BaseRateForKm { get; set; }  // Base rate for distance
        public double BaseRateForKg { get; set; }  // Base rate for weight
        public double BaseRateForVolume { get; set; }  // Base rate for volume
        public double ExpressRateMultiplier { get; set; }  // Multiplier for express delivery
        public double OvernightRateMultiplier { get; set; }  // Multiplier for overnight delivery
        public double ExpressDeliveryTimeMultiplier { get; set; }  // Multiplier for express delivery time
        public double OvernightDeliveryTimeMultiplier { get; set; }  // Multiplier for overnight delivery time
        public ICollection<Shipment> Shipments { get; set; } = [];
        private ShipmentRate(string name,string? description,decimal baseRate,double minWeight, double maxWeight,double minDistance, double maxDistance,double minVolume, double maxVolume,double baseRateForKm,double baseRateForKg,double baseRateForVolume,double expressRateMultiplier,double overnightRateMultiplier, double expressDeliveryTimeMultiplier,double overnightDeliveryTimeMultiplier)
        {
            Name = name;
            Description = description;
            BaseRate = baseRate;
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
            MinVolume = minVolume;
            MaxVolume = maxVolume;
            BaseRateForKm = baseRateForKm;
            BaseRateForKg = baseRateForKg;
            BaseRateForVolume = baseRateForVolume;
            ExpressRateMultiplier = expressRateMultiplier;
            OvernightRateMultiplier = overnightRateMultiplier;
            ExpressDeliveryTimeMultiplier = expressDeliveryTimeMultiplier;
            OvernightDeliveryTimeMultiplier = overnightDeliveryTimeMultiplier;
        }

        public static ShipmentRate Create(string name, string? description, decimal baseRate, double minWeight, double maxWeight, double minDistance, double maxDistance, double minVolume, double maxVolume, double baseRateForKm, double baseRateForKg, double baseRateForVolume, double expressRateMultiplier, double overnightRateMultiplier, double expressDeliveryTimeMultiplier, double overnightDeliveryTimeMultiplier)
        {
            return new ShipmentRate(
                name,
                description,
                baseRate,
                minWeight,
                maxWeight,
                minDistance,
                maxDistance,
                minVolume,
                maxVolume,
                baseRateForKm,
                baseRateForKg,
                baseRateForVolume,
                expressRateMultiplier,
                overnightRateMultiplier,
                expressDeliveryTimeMultiplier,
                overnightDeliveryTimeMultiplier
            );
        }
    }
}
