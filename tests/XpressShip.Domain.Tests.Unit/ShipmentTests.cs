using XpressShip.Domain.Entities.Users;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Exceptions;
using FluentAssertions;
using XpressShip.Tests.Common;

namespace XpressShip.Domain.Tests.Unit
{
    public class ShipmentTests
    {
        [Fact]
        public void Create_ShouldInitializeWithPendingStatus()
        {
            // Arrange
            double weight = 10.5;
            string dimensions = "10x20x30";
            ShipmentMethod method = ShipmentMethod.Standard;
            string note = "Handle with care";

            // Act
            var shipment = Shipment.Create(weight, dimensions, method, note);

            // Assert
            shipment.Should().NotBeNull();
            shipment.Status.Should().Be(ShipmentStatus.Pending);
            shipment.Weight.Should().Be(weight);
            shipment.Dimensions.Should().Be(dimensions);
            shipment.Method.Should().Be(method);
            shipment.Note.Should().Be(note);
            shipment.TrackingNumber.Should().NotBeEmpty();
        }

        [Fact]
        public void Update_ShouldModifyShipmentProperties()
        {
            // Arrange
            var shipment = DataFactory.CreateFakeShipment();

            double newWeight = 7.5;
            string newDimensions = "10x10x10";
            string newMethod = ShipmentMethod.Overnight.ToString();
            string newNote = "Handle with care";

            // Act
            shipment.Update(newWeight, newDimensions, newMethod, newNote);

            // Assert
            shipment.Weight.Should().Be(newWeight);
            shipment.Dimensions.Should().Be(newDimensions);
            shipment.Method.Should().Be(ShipmentMethod.Overnight);
            shipment.Note.Should().Be(newNote);
        }

        [Fact]
        public void CalculateShippingCost_ShouldCorrectlyCalculateShippingCost()
        {
            // Arrange
            double weight = 4.5;
            string dimensions = "2x3x3";
            ShipmentMethod method = ShipmentMethod.Standard;
            string note = "Handle with care";
            var shipment = Shipment.Create(weight, dimensions, method, note);

            var name = "Small Package - Local";
            var desc = "Rate for small packages within local regions.";
            var baseRate = 10.00m;
            var minWeight = 0;
            var maxWeight = 5;
            var minDistance = 0;
            var maxDistance = 100;
            var minVolume = 0;
            var maxVolume = 20;
            var baseRateForKm = 0.05;
            var baseRateForKg = 1.5;
            var baseRateForVolume = 2.0;
            var expressRateMultiplier = 1.2;
            var overnightRateMultiplier = 1.5;
            var expressDeliveryMultiplier = 0.8;
            var overnightDeliveryMultiplier = 0.5;

            var shipmentRate = ShipmentRate.Create(name, desc, baseRate, minWeight, maxWeight, minDistance, maxDistance, minVolume, maxVolume, baseRateForKm, baseRateForKg, baseRateForVolume, expressRateMultiplier, overnightRateMultiplier, expressDeliveryMultiplier, overnightDeliveryMultiplier);

            var origin = Address.Create("XYZ1000", "St.John", 80, -170);
            var destination = Address.Create("ZYX1000", "St.James", 80, -175);

            var destinationCountry = Country.Create("Azerbaijan", "AZE", @"AZ\s\d{4}$", 20);

            var destinarionCity = City.Create("Baku", destinationCountry);

            destination.City = destinarionCity;

            shipment.Rate = shipmentRate;
            shipment.OriginAddress = origin;
            shipment.DestinationAddress = destination;

            // Act
            var answer = shipment.CalculateShippingCost();

            // Assert
            answer.Should().NotBe(0);
        }

        [Theory]
        [InlineData(100, 80)]
        [InlineData(80, 64)]
        [InlineData(40, 32)]
        [InlineData(400, 320)]
        public void ApplyTax_ShouldApplyTaxToTheTotalCost(int totalCost, int taxAppliedCost)
        {
            // Arrange
            const int taxPercentage = 20;
            var shipment = DataFactory.CreateFakeShipment();

            var destination = Address.Create("ZYX1000", "St.James", 80, -175);
            var destinationCountry = Country.Create("Azerbaijan", "AZE", @"AZ\s\d{4}$", taxPercentage);
            var destinarionCity = City.Create("Baku", destinationCountry);

            destination.City = destinarionCity;
            shipment.DestinationAddress = destination;

            // Act
            var answer = shipment.ApplyTax(totalCost);

            //Assert
            answer.Should().Be(taxAppliedCost);
        }
        [Fact]
        public void ApplyTax_WhenNotGivingDestination_ShouldThrowException()
        {
            // Arrange
            var shipment = DataFactory.CreateFakeShipment();

            // Act
            var action = () => shipment.ApplyTax(100);

            //Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Fact]
        public void MakeDelivered_ShouldUpdateStatusAndClearEstimatedDate()
        {
            // Arrange
            var shipment = DataFactory.CreateFakeShipment();

            shipment.OriginAddress = DataFactory.CreateFakeAddress();
            shipment.DestinationAddress = DataFactory.CreateFakeAddress();

            shipment.Rate = DataFactory.CreateFakeShipmentRate();

            shipment.MakeShipped();

            // Act
            shipment.MakeDelivered();

            // Assert
            shipment.Status.Should().Be(ShipmentStatus.Delivered);
            shipment.EstimatedDate.Should().BeNull();
        }

        [Fact]
        public void MakeShipped_ShouldSetStatusAndEstimatedDate()
        {
            // Arrange
            var shipment = DataFactory.CreateFakeShipment();

            shipment.OriginAddress = DataFactory.CreateFakeAddress();
            shipment.DestinationAddress = DataFactory.CreateFakeAddress();

            shipment.Rate = DataFactory.CreateFakeShipmentRate();

            // Act
            shipment.MakeShipped();

            // Assert
            shipment.Status.Should().Be(ShipmentStatus.Shipped);
            shipment.EstimatedDate.Should().NotBeNull();
        }

        [Fact]
        public void CalculateVolume_ShouldReturnCorrectVolume()
        {
            // Arrange
            string dimensions = "2x3x4";

            // Act
            int volume = Shipment.CalculateVolume(dimensions);

            // Assert
            volume.Should().Be(24);
        }

        [Fact]
        public void ValidateDimensions_InvalidFormat_ShouldThrowException()
        {
            // Arrange
            string invalidDimensions = "10-20-30";

            // Act
            var action = () => Shipment.ValidateDimensions(invalidDimensions);

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Fact]
        public void GetRecipient_ShouldReturnCorrectNameAndEmail_ForSender()
        {
            // Arrange
            var sender = Sender.Create("John", "Doe", "johnDoe", "john@example.com", "+994514566778");

            var shipment = DataFactory.CreateFakeShipment();

            shipment.Sender = sender;

            // Act
            var (name, email) = shipment.GetRecipient();

            // Assert
            name.Should().Be("johnDoe");
            email.Should().Be("john@example.com");
        }

        [Fact]
        public void GetRecipient_ShouldReturnCorrectNameAndEmail_ForApiClient()
        {
            // Arrange
            var (client, _) = ApiClient.Create("Test Company", "testCompany@example.com");

            var shipment = DataFactory.CreateFakeShipment();

            shipment.ApiClient = client;

            // Act
            var (name, email) = shipment.GetRecipient();

            // Assert
            name.Should().Be("Test Company");
            email.Should().Be("testCompany@example.com");
        }

        [Fact]
        public void GetRecipient_ShouldThrowException_WhenBothSenderAndApiClientAreNotNull()
        {
            // Arrange
            var sender = DataFactory.CreateFakeSender();
            var (client, _) = DataFactory.CreateFakeApiClient();

            var shipment = DataFactory.CreateFakeShipment();

            shipment.ApiClient = client;
            shipment.Sender = sender;

            // Act
            var action = () => shipment.GetRecipient();

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Fact]
        public void GetRecipient_ShouldThrowException_WhenBothSenderAndApiClientAreNull()
        {
            // Arrange
            var shipment = DataFactory.CreateFakeShipment();

            // Act
            var action = () => shipment.GetRecipient();

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Fact]
        public void GenerateTrackingNumber_ShouldReturnValidFormat()
        {
            // Act
            var shipment = DataFactory.CreateFakeShipment();

            string trackingNumber = shipment.TrackingNumber;

            // Assert
            trackingNumber.Should().StartWith("TRK-");
            trackingNumber.Should().MatchRegex(@"TRK-\d{8}-[A-Z0-9]{8}");
        }
    }
}
