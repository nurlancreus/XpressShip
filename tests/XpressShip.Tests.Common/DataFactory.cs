using Bogus;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Users;
using XpressShip.Domain.Enums;

namespace XpressShip.Tests.Common
{
    public class DataFactory
    {
        private static readonly Faker _faker = new();

        public static Shipment CreateFakeShipment()
        {
            return Shipment.Create(
                _faker.Random.Double(0.5, 50), // Random weight
                $"{_faker.Random.Int(5, 50)}x{_faker.Random.Int(5, 50)}x{_faker.Random.Int(5, 50)}", // Random dimensions
                _faker.PickRandom<ShipmentMethod>(), // Random shipment method
                _faker.Lorem.Sentence() // Random note
            );
        }

        public static Sender CreateFakeSender()
        {
            return Sender.Create(
                _faker.Name.FirstName(),
                _faker.Name.LastName(),
                _faker.Internet.UserName(),
                _faker.Internet.Email(),
                _faker.Phone.PhoneNumber()
            );
        }

        public static (ApiClient, string) CreateFakeApiClient()
        {
            return ApiClient.Create(
                _faker.Company.CompanyName(),
                _faker.Internet.Email()
            );
        }

        public static Address CreateFakeAddress()
        {
            return Address.Create(
                _faker.Address.ZipCode(),
                _faker.Address.City(),
                _faker.Address.Latitude(),
                _faker.Address.Longitude()
            );
        }

        public static ShipmentRate CreateFakeShipmentRate()
        {
            return ShipmentRate.Create(
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
                _faker.Random.Double(0.5, 1.5),  // Express Delivery Time Multiplier
                _faker.Random.Double(0.5, 1),  // Overnight Delivery Time Multiplier
                _faker.Random.Double(0.5, 2)  // Base Rate per Volume
    );
        }
    }
}
