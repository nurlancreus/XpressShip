using XpressShip.Domain.Entities.Base;
using XpressShip.Domain.Enums;

namespace XpressShip.Domain.Entities
{
    public class ShipmentRate : BaseEntity
    {
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
        public ICollection<Shipment> Shipments { get; set; } = [];

        private ShipmentRate()
        {

        }
        private ShipmentRate(string name, string? description, decimal baseRate, double minWeight, double maxWeight, double minDistance, double maxDistance, double minVolume, double maxVolume, double baseRateForKm, double baseRateForKg, double baseRateForVolume, double expressRateMultiplier, double overnightRateMultiplier, double expressDeliveryTimeMultiplier, double overnightDeliveryTimeMultiplier)
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
            return new ShipmentRate(name, description, baseRate, minWeight, maxWeight, minDistance, maxDistance, minVolume, maxVolume, baseRateForKm, baseRateForKg, baseRateForVolume, expressRateMultiplier, overnightRateMultiplier, expressDeliveryTimeMultiplier, overnightDeliveryTimeMultiplier);
        }
    }
}
