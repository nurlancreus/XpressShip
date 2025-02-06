using ShipmentRateEntity = XpressShip.Domain.Entities.ShipmentRate;
using DataConstants = XpressShip.Tests.Common.Constants.Constants;
using Bogus;

namespace XpressShip.Tests.Common.Factories
{
    public static partial class Factory
    {
        public static class ShipmentRate
        {
            private static readonly Faker _faker = new();

            public static ShipmentRateEntity CreateFakeShipmentRate()
            {
                return ShipmentRateEntity.Create(
                    _faker.Commerce.ProductName(),  // Name
                    _faker.Lorem.Sentence(),  // Description
                    _faker.Random.Decimal(5, 50),  // Base Rate
                    _faker.Random.Double(0, 5),  // Min Weight
                    _faker.Random.Double(5, 50),  // Max Weight
                    _faker.Random.Double(0, 100),  // Min Distance
                    _faker.Random.Double(100, 1000),  // Max Distance
                    _faker.Random.Double(0, 10),  // Min Volume
                    _faker.Random.Double(10, 100),  // Max Volume
                    _faker.Random.Double(0.5, 5),  // Base Rate per KG
                    _faker.Random.Double(0.5, 0.5),  // Base Rate per KM
                    _faker.Random.Double(0.5, 2),  // Express Rate Multiplier
                    _faker.Random.Double(0.5, 3),  // Overnight Rate Multiplier
                    _faker.Random.Double(0.5, 0.8),  // Express Delivery Time Multiplier
                    _faker.Random.Double(0.1, 0.5),  // Overnight Delivery Time Multiplier
                    _faker.Random.Double(0.5, 2)  // Base Rate per Volume
        );
            }
            public static ShipmentRateEntity GenerateSmallRate()
            {
                var name = DataConstants.ShipmentRate.Name;
                var desc = DataConstants.ShipmentRate.Description;
                var baseRate = DataConstants.ShipmentRate.BaseRate;
                var minWeight = DataConstants.ShipmentRate.MinWeight;
                var maxWeight = DataConstants.ShipmentRate.MaxWeight;
                var minDistance = DataConstants.ShipmentRate.MinDistance;
                var maxDistance = DataConstants.ShipmentRate.MaxDistance;
                var minVolume = DataConstants.ShipmentRate.MinVolume;
                var maxVolume = DataConstants.ShipmentRate.MaxVolume;
                var baseRateForKm = DataConstants.ShipmentRate.BaseRateForKm;
                var baseRateForKg = DataConstants.ShipmentRate.BaseRateForKg;
                var baseRateForVolume = DataConstants.ShipmentRate.BaseRateForVolume;
                var expressRateMultiplier = DataConstants.ShipmentRate.ExpressRateMultiplier;
                var overnightRateMultiplier = DataConstants.ShipmentRate.OvernightRateMultiplier;
                var expressDeliveryMultiplier = DataConstants.ShipmentRate.ExpressDeliveryMultiplier;
                var overnightDeliveryMultiplier = DataConstants.ShipmentRate.OvernightDeliveryMultiplier;

                return ShipmentRateEntity.Create(name, desc, baseRate, minWeight, maxWeight, minDistance, maxDistance, minVolume, maxVolume, baseRateForKm, baseRateForKg, baseRateForVolume, expressRateMultiplier, overnightRateMultiplier, expressDeliveryMultiplier, overnightDeliveryMultiplier);
            }

            public static ShipmentRateEntity GenerateMediumRate()
            {
                var name = DataConstants.MediumShipmentRate.Name;
                var desc = DataConstants.MediumShipmentRate.Description;
                var baseRate = DataConstants.MediumShipmentRate.BaseRate;
                var minWeight = DataConstants.MediumShipmentRate.MinWeight;
                var maxWeight = DataConstants.MediumShipmentRate.MaxWeight;
                var minDistance = DataConstants.MediumShipmentRate.MinDistance;
                var maxDistance = DataConstants.MediumShipmentRate.MaxDistance;
                var minVolume = DataConstants.MediumShipmentRate.MinVolume;
                var maxVolume = DataConstants.MediumShipmentRate.MaxVolume;
                var baseRateForKm = DataConstants.MediumShipmentRate.BaseRateForKm;
                var baseRateForKg = DataConstants.MediumShipmentRate.BaseRateForKg;
                var baseRateForVolume = DataConstants.MediumShipmentRate.BaseRateForVolume;
                var expressRateMultiplier = DataConstants.MediumShipmentRate.ExpressRateMultiplier;
                var overnightRateMultiplier = DataConstants.MediumShipmentRate.OvernightRateMultiplier;
                var expressDeliveryMultiplier = DataConstants.MediumShipmentRate.ExpressDeliveryMultiplier;
                var overnightDeliveryMultiplier = DataConstants.MediumShipmentRate.OvernightDeliveryMultiplier;

                return ShipmentRateEntity.Create(name, desc, baseRate, minWeight, maxWeight, minDistance, maxDistance, minVolume, maxVolume, baseRateForKm, baseRateForKg, baseRateForVolume, expressRateMultiplier, overnightRateMultiplier, expressDeliveryMultiplier, overnightDeliveryMultiplier);
            }

            public static ShipmentRateEntity GenerateLargeRate()
            {
                var name = DataConstants.LargeShipmentRate.Name;
                var desc = DataConstants.LargeShipmentRate.Description;
                var baseRate = DataConstants.LargeShipmentRate.BaseRate;
                var minWeight = DataConstants.LargeShipmentRate.MinWeight;
                var maxWeight = DataConstants.LargeShipmentRate.MaxWeight;
                var minDistance = DataConstants.LargeShipmentRate.MinDistance;
                var maxDistance = DataConstants.LargeShipmentRate.MaxDistance;
                var minVolume = DataConstants.LargeShipmentRate.MinVolume;
                var maxVolume = DataConstants.LargeShipmentRate.MaxVolume;
                var baseRateForKm = DataConstants.LargeShipmentRate.BaseRateForKm;
                var baseRateForKg = DataConstants.LargeShipmentRate.BaseRateForKg;
                var baseRateForVolume = DataConstants.LargeShipmentRate.BaseRateForVolume;
                var expressRateMultiplier = DataConstants.LargeShipmentRate.ExpressRateMultiplier;
                var overnightRateMultiplier = DataConstants.LargeShipmentRate.OvernightRateMultiplier;
                var expressDeliveryMultiplier = DataConstants.LargeShipmentRate.ExpressDeliveryMultiplier;
                var overnightDeliveryMultiplier = DataConstants.LargeShipmentRate.OvernightDeliveryMultiplier;

                return ShipmentRateEntity.Create(name, desc, baseRate, minWeight, maxWeight, minDistance, maxDistance, minVolume, maxVolume, baseRateForKm, baseRateForKg, baseRateForVolume, expressRateMultiplier, overnightRateMultiplier, expressDeliveryMultiplier, overnightDeliveryMultiplier);
            }
        }
    }
}
