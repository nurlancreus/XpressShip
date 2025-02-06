using XpressShip.Domain.Enums;
using ShipmentEntity = XpressShip.Domain.Entities.Shipment;
using DataConstants = XpressShip.Tests.Common.Constants.Constants;
using Bogus;

namespace XpressShip.Tests.Common.Factories
{
    public static partial class Factory
    {
        public static class Shipment
        {
            private static readonly Faker _faker = new();

            public static ShipmentEntity CreateFakeShipment()
            {
                return ShipmentEntity.Create(
                    _faker.Random.Double(0.5, 50), // Random weight
                    $"{_faker.Random.Int(5, 50)}x{_faker.Random.Int(5, 50)}x{_faker.Random.Int(5, 50)}", // Random dimensions
                    _faker.PickRandom<ShipmentMethod>(), // Random shipment method
                    _faker.Lorem.Sentence() // Random note
                );
            }
            public static ShipmentEntity GenerateSmallShipment()
            {
                double weight = DataConstants.Shipment.Weight;
                string dimensions = DataConstants.Shipment.Dimensions;
                ShipmentMethod method = DataConstants.Shipment.Method;
                string note = DataConstants.Shipment.Note;

                return ShipmentEntity.Create(weight, dimensions, method, note);
            }

            public static ShipmentEntity GenerateMediumShipment()
            {
                double weight = DataConstants.MediumShipment.Weight;
                string dimensions = DataConstants.MediumShipment.Dimensions;
                ShipmentMethod method = DataConstants.MediumShipment.Method;
                string note = DataConstants.MediumShipment.Note;

                return ShipmentEntity.Create(weight, dimensions, method, note);
            }

            public static ShipmentEntity GenerateLargeShipment()
            {
                double weight = DataConstants.LargeShipment.Weight;
                string dimensions = DataConstants.LargeShipment.Dimensions;
                ShipmentMethod method = DataConstants.LargeShipment.Method;
                string note = DataConstants.LargeShipment.Note;

                return ShipmentEntity.Create(weight, dimensions, method, note);
            }
        }
    }
}
