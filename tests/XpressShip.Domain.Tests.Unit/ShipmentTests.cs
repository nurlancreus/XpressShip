using XpressShip.Domain.Entities.Users;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Exceptions;
using FluentAssertions;
using DataConstants = XpressShip.Tests.Common.Constants.Constants;
using XpressShip.Tests.Common.Factories;

namespace XpressShip.Domain.Tests.Unit
{
    public class ShipmentTests
    {
        [Fact]
        public void Create_ShouldInitializeWithPendingStatus()
        {
            // Arrange
            double weight = DataConstants.Shipment.Weight;
            string dimensions = DataConstants.Shipment.Dimensions;
            ShipmentMethod method = DataConstants.Shipment.Method;
            string note = DataConstants.Shipment.Note;

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
            var shipment = Factory.Shipment.CreateFakeShipment();

            double newWeight = DataConstants.Shipment.NewWeight;
            string newDimensions = DataConstants.Shipment.NewDimensions;
            string newMethod = DataConstants.Shipment.NewMethod.ToString();
            string newNote = DataConstants.Shipment.NewNote;

            // Act
            shipment.Update(newWeight, newDimensions, newMethod, newNote);

            // Assert
            shipment.Weight.Should().Be(newWeight);
            shipment.Dimensions.Should().Be(newDimensions);
            shipment.Method.Should().Be(ShipmentMethod.Overnight);
            shipment.Note.Should().Be(newNote);
        }

        [Fact]
        public void CalculateShippingCost_WhenApplyingSmallRateForSmallShipment_ShouldCorrectlyCalculateShippingCost()
        {
            // Arrange
            var shipment = Factory.Shipment.GenerateSmallShipment();

            var shipmentRate = Factory.ShipmentRate.GenerateSmallRate();

            var origin = Factory.Address.GenerateOriginAddress();
            var destination = Factory.Address.GenerateDestinationAddress();

            shipment.Rate = shipmentRate;
            shipment.OriginAddress = origin;
            shipment.DestinationAddress = destination;

            // Act
            var answer = shipment.CalculateShippingCost();

            // Assert
            answer.Should().NotBe(0);
        }

        [Fact]
        public void CalculateShippingCost_WhenApplyingMediumRateForMediumShipment_ShouldCorrectlyCalculateShippingCost()
        {
            // Arrange
            var shipment = Factory.Shipment.GenerateMediumShipment();

            var shipmentRate = Factory.ShipmentRate.GenerateMediumRate();

            var origin = Factory.Address.GenerateOriginAddress();
            var destination = Factory.Address.GenerateDestinationAddress();

            shipment.Rate = shipmentRate;
            shipment.OriginAddress = origin;
            shipment.DestinationAddress = destination;

            // Act
            var answer = shipment.CalculateShippingCost();

            // Assert
            answer.Should().NotBe(0);
        }

        [Fact]
        public void CalculateShippingCost_WhenApplyingLargeRateForLargeShipment_ShouldCorrectlyCalculateShippingCost()
        {
            // Arrange
            var shipment = Factory.Shipment.GenerateLargeShipment();

            var shipmentRate = Factory.ShipmentRate.GenerateLargeRate();

            var origin = Factory.Address.GenerateOriginAddress();
            var destination = Factory.Address.GenerateDestinationAddress();

            shipment.Rate = shipmentRate;
            shipment.OriginAddress = origin;
            shipment.DestinationAddress = destination;

            // Act
            var answer = shipment.CalculateShippingCost();

            // Assert
            answer.Should().NotBe(0);
        }

        [Fact]
        public void CalculateShippingCost_WhenApplyingMediumRateForLargeShipment_ShouldThrowException()
        {
            // Arrange
            var shipment = Factory.Shipment.GenerateLargeShipment();

            var shipmentRate = Factory.ShipmentRate.GenerateMediumRate();

            var origin = Factory.Address.GenerateOriginAddress();
            var destination = Factory.Address.GenerateDestinationAddress();

            shipment.Rate = shipmentRate;
            shipment.OriginAddress = origin;
            shipment.DestinationAddress = destination;

            // Act
            var action = () => shipment.CalculateShippingCost();

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Fact]
        public void CalculateShippingCost_WhenApplyingSmallRateForMediumShipment_ShouldThrowException()
        {
            // Arrange
            var shipment = Factory.Shipment.GenerateMediumShipment();

            var shipmentRate = Factory.ShipmentRate.GenerateSmallRate();

            var origin = Factory.Address.GenerateOriginAddress();
            var destination = Factory.Address.GenerateDestinationAddress();

            shipment.Rate = shipmentRate;
            shipment.OriginAddress = origin;
            shipment.DestinationAddress = destination;

            // Act
            var action = () => shipment.CalculateShippingCost();

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Theory]
        [InlineData(100, 80)]
        [InlineData(80, 64)]
        [InlineData(40, 32)]
        [InlineData(400, 320)]
        public void ApplyTax_ShouldApplyTaxToTheTotalCost(int totalCost, int taxAppliedCost)
        {
            // Arrange
            var shipment = Factory.Shipment.CreateFakeShipment();

            var destination = Factory.Address.GenerateDestinationAddress(taxPercentage: 20);
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
            var shipment = Factory.Shipment.CreateFakeShipment();

            // Act
            var action = () => shipment.ApplyTax(100);

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Theory]
        [InlineData(100)]
        [InlineData(80)]
        [InlineData(200)]
        public void ValidateDistance_WhenDistanceIsNotOutOfRange_ShouldReturnTrue(double distance)
        {
            // Arrange
            var rate = Factory.ShipmentRate.GenerateMediumRate(); // Distance: 0 - 200

            // Act
            var answer = Shipment.ValidateDistance(distance, rate);

            // Assert
            answer.Should().BeTrue();
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(-80)]
        [InlineData(201)]
        public void ValidateDistance_WhenDistanceIsOutOfRange_ShouldThrowException(double distance)
        {
            // Arrange
            var rate = Factory.ShipmentRate.GenerateMediumRate(); // Distance: 0 - 200

            // Act
            var action = () => Shipment.ValidateDistance(distance, rate);

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(-80)]
        [InlineData(201)]
        public void ValidateDistance_WhenDistanceIsOutOfRangeAndThrowExceptionIsFalse_ShouldReturnFalse(double distance)
        {
            // Arrange
            var rate = Factory.ShipmentRate.GenerateMediumRate(); // Distance: 0 - 200

            // Act
            var answer = Shipment.ValidateDistance(distance, rate, false);

            // Assert
            answer.Should().BeFalse();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(8)]
        [InlineData(20)]
        public void ValidateWeight_WhenWeightIsNotOutOfRange_ShouldReturnTrue(double weight)
        {
            // Arrange
            var rate = Factory.ShipmentRate.GenerateMediumRate(); // Weight: 0 - 20

            // Act
            var answer = Shipment.ValidateWeight(weight, rate);

            // Assert
            answer.Should().BeTrue();
        }

        [Theory]
        [InlineData(100)]
        [InlineData(-1)]
        [InlineData(21)]
        public void ValidateWeight_WhenWeightIsOutOfRange_ShouldThrowException(double weight)
        {
            // Arrange
            var rate = Factory.ShipmentRate.GenerateMediumRate(); // Weight: 0 - 20

            // Act
            var action = () => Shipment.ValidateWeight(weight, rate);

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Theory]
        [InlineData(100)]
        [InlineData(-1)]
        [InlineData(21)]
        public void ValidateWeight_WhenWeightIsOutOfRangeAndThrowExceptionIsFalse_ShouldReturnFalse(double weight)
        {
            // Arrange
            var rate = Factory.ShipmentRate.GenerateMediumRate(); // Weight: 0 - 20

            // Act
            var answer = Shipment.ValidateWeight(weight, rate, false);

            // Assert
            answer.Should().BeFalse();
        }

        [Theory]
        [InlineData(30)]
        [InlineData(20)]
        public void ValidateVolume_WhenVolumeIsNotOutOfRange_ShouldReturnTrue(double volume)
        {
            // Arrange
            var rate = Factory.ShipmentRate.GenerateMediumRate(); // Volume: 20 - 50

            // Act
            var answer = Shipment.ValidateVolume(volume, rate);

            // Assert
            answer.Should().BeTrue();
        }

        [Theory]
        [InlineData(100)]
        [InlineData(-1)]
        [InlineData(51)]
        public void ValidateVolume_WhenVolumeIsOutOfRange_ShouldThrowException(double voulme)
        {
            // Arrange
            var rate = Factory.ShipmentRate.GenerateMediumRate(); // Volume: 20 - 50

            // Act
            var action = () => Shipment.ValidateVolume(voulme, rate);

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Theory]
        [InlineData(100)]
        [InlineData(-1)]
        [InlineData(51)]
        public void ValidateVolume_WhenVolumeIsOutOfRangeAndThrowExceptionIsFalse_ShouldReturnFalse(double voulme)
        {
            // Arrange
            var rate = Factory.ShipmentRate.GenerateMediumRate(); // Volume: 20 - 50

            // Act
            var answer = Shipment.ValidateVolume(voulme, rate, false);

            // Assert
            answer.Should().BeFalse();
        }

        [Fact]
        public void ValidateDimensions_WhenValidFormat_ShouldReturnTrue()
        {
            // Arrange
            const string validDimensions = "10x20x30";

            // Act
            var answer = Shipment.ValidateDimensions(validDimensions);

            // Assert
            answer.Should().BeTrue();
        }

        [Theory]
        [InlineData("10-20-30")]
        [InlineData("10.20.30")]
        [InlineData("10x20x30x40")]
        public void ValidateDimensions_WhenInvalidFormat_ShouldThrowException(string invalidDimensions)
        {
            // Arrange & Act
            var action = () => Shipment.ValidateDimensions(invalidDimensions);

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Theory]
        [InlineData("10-20-30")]
        [InlineData("10.20.30")]
        [InlineData("10x20x30x40")]
        public void ValidateDimensions_WhenInvalidFormatAndThrowExceptionIsFalse_ShouldReturnFalse(string invalidDimensions)
        {
            // Arrange & Act
            var answer = Shipment.ValidateDimensions(invalidDimensions, false);

            // Assert
            answer.Should().BeFalse();
        }

        [Fact]
        public void MakeDelivered_ShouldUpdateStatusAndClearEstimatedDate()
        {
            // Arrange
            var shipment = Factory.Shipment.CreateFakeShipment();

            shipment.OriginAddress = Factory.Address.CreateFakeAddress();
            shipment.DestinationAddress = Factory.Address.CreateFakeAddress();

            shipment.Rate = Factory.ShipmentRate.CreateFakeShipmentRate();

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
            var shipment = Factory.Shipment.CreateFakeShipment();

            shipment.OriginAddress = Factory.Address.CreateFakeAddress();
            shipment.DestinationAddress = Factory.Address.CreateFakeAddress();

            shipment.Rate = Factory.ShipmentRate.CreateFakeShipmentRate();

            // Act
            shipment.MakeShipped();

            // Assert
            shipment.Status.Should().Be(ShipmentStatus.Shipped);
            shipment.EstimatedDate.Should().NotBeNull();
        }

        [Theory]
        [InlineData("2x3x4", 24)]
        [InlineData("3x3x4", 36)]
        [InlineData("1x1x1", 1)]
        public void CalculateVolume_WhenProvidingValidDimensions_ShouldReturnCorrectVolume(string dimensions, int answer)
        {
            // Arrange & Act
            int volume = Shipment.CalculateVolume(dimensions);

            // Assert
            volume.Should().Be(answer);
        }

        [Fact]
        public void GetRecipient_ShouldReturnCorrectNameAndEmail_ForSender()
        {
            // Arrange
            var sender = Sender.Create("John", "Doe", "johnDoe", "john@example.com", "+994514566778");

            var shipment = Factory.Shipment.CreateFakeShipment();

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

            var shipment = Factory.Shipment.CreateFakeShipment();

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
            var sender = Factory.Sender.GenerateSender();
            var (client, _) = Factory.ApiClient.GenerateApiClient();

            var shipment = Factory.Shipment.GenerateSmallShipment();

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
            var shipment = Factory.Shipment.CreateFakeShipment();

            // Act
            var action = () => shipment.GetRecipient();

            // Assert
            action.Should().ThrowExactly<XpressShipException>();
        }

        [Fact]
        public void GenerateTrackingNumber_ShouldReturnValidFormat()
        {
            // Act
            var shipment = Factory.Shipment.CreateFakeShipment();

            string trackingNumber = shipment.TrackingNumber;

            // Assert
            trackingNumber.Should().StartWith("TRK-");
            trackingNumber.Should().MatchRegex(@"TRK-\d{8}-[A-Z0-9]{8}");
        }
    }
}
