using FluentAssertions;
using XpressShip.Domain.Entities;
using XpressShip.Tests.Common.Factories;

namespace XpressShip.Domain.Tests.Unit
{
    public class AddressTests
    {
        [Fact]
        public void CalculateDistance_WhenProvidingValidAddress_ShouldCalculateDistance()
        {
            // Arrange
            var origin = Factory.Address.GenerateOriginAddress();
            var destination = Factory.Address.GenerateDestinationAddress();

            // Act
            var answer = origin.CalculateDistance(destination);

            // Assert
            answer.Should().NotBe(0);
        }

        [Theory]
        [InlineData(90)]
        [InlineData(-90)]
        [InlineData(0)]
        public void IsValidLatitude_WhenProvidingValidLat_ShouldReturnTrue(double latitude)
        {
            // Arrange & Act
            var answer = Address.IsValidLatitude(latitude);

            // Assert
            answer.Should().BeTrue();
        }

        [Theory]
        [InlineData(91)]
        [InlineData(-91)]
        [InlineData(100)]
        public void IsValidLatitude_WhenProvidingInValidLat_ShouldThrowException(double latitude)
        {
            // Arrange & Act
            var action = () => Address.IsValidLatitude(latitude);

            // Assert
            action.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData(91)]
        [InlineData(-91)]
        [InlineData(100)]
        public void IsValidLatitude_WhenProvidingInValidLatAndThrowExceptionIsFalse_ShouldReturnFalse(double latitude)
        {
            // Arrange & Act
            var answer = Address.IsValidLatitude(latitude, false);

            // Assert
            answer.Should().BeFalse();
        }

        [Theory]
        [InlineData(180)]
        [InlineData(-180)]
        [InlineData(0)]
        public void IsValidLongitude_WhenProvidingValidLon_ShouldReturnTrue(double longitude)
        {
            // Arrange & Act
            var answer = Address.IsValidLongitude(longitude);

            // Assert
            answer.Should().BeTrue();
        }

        [Theory]
        [InlineData(181)]
        [InlineData(-181)]
        public void IsValidLongitude_WhenProvidingInValidLon_ShouldThrowException(double longitude)
        {
            // Arrange & Act
            var action = () => Address.IsValidLongitude(longitude);

            // Assert
            action.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData(181)]
        [InlineData(-181)]
        public void IsValidLongitude_WhenProvidingInValidLonAndThrowExceptionIsFalse_ShouldReturnFalse(double longitude)
        {
            // Arrange & Act
            var answer = Address.IsValidLongitude(longitude, false);

            // Assert
            answer.Should().BeFalse();
        }
    }
}
