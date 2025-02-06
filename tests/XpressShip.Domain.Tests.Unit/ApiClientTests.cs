using FluentAssertions;
using XpressShip.Domain.Entities;
using XpressShip.Tests.Common.Factories;
using DataConstants = XpressShip.Tests.Common.Constants.Constants;

namespace XpressShip.Domain.Tests.Unit
{
    public class ApiClientTests
    {
        [Fact]
        public void Create_ShouldInitializeWithRawSecretKey()
        {
            // Arrange
            var companyName = DataConstants.ApiClient.CompanyName;
            var email = DataConstants.ApiClient.Email;

            // Act
            var (client, rawSecretKey) = ApiClient.Create(companyName, email);

            // Assert
            client.Should().NotBeNull();
            rawSecretKey.Should().NotBeEmpty();
            client.CompanyName.Should().Be(companyName);
            client.Email.Should().Be(email);
            client.ApiKey.Should().NotBeEmpty();
            client.SecretKey.Should().NotBeEmpty();
            client.IsActive.Should().BeFalse();
        }

        [Fact]
        public void VerifySecretKey_WhenRawSecretAndHashedIsEqual_ShouldReturnTrue()
        {
            // Arrange
            var (client, rawSecretKey) = Factory.ApiClient.GenerateApiClient();

            // Act
            var answer = ApiClient.VerifySecretKey(rawSecretKey, client.SecretKey);

            // Assert
            answer.Should().BeTrue();
        }

        [Fact]
        public void VerifySecretKey_WhenRawSecretAndHashedIsNotEqual_ShouldReturnFalse()
        {
            // Arrange
            var (client, _) = Factory.ApiClient.GenerateApiClient();

            // Act
            var answer = ApiClient.VerifySecretKey(string.Empty, client.SecretKey);

            // Assert
            answer.Should().BeFalse();
        }

        [Fact]
        public void Toggle_WhenIsActiveTrue_ShouldToggleAndSetDeActivatedAtNull()
        {
            // Arrange
            var (client, _) = Factory.ApiClient.GenerateApiClient();

            // Act
            client.Toggle();

            // Assert
            client.IsActive.Should().BeTrue();
            client.DeActivatedAt.Should().BeNull();
        }

        [Fact]
        public void Toggle_WhenIsActiveFalse_ShouldToggleAndSetDeActivatedAtNotNull()
        {
            // Arrange
            var (client, _) = Factory.ApiClient.GenerateApiClient();
            client.IsActive = true;

            // Act
            client.Toggle();

            // Assert
            client.IsActive.Should().BeFalse();
            client.DeActivatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateApiKey_WhenUpdateApiKey_ShouldChange()
        {
            // Arrange
            var (client, _) = Factory.ApiClient.GenerateApiClient();
            var oldApiKey = client.ApiKey;

            // Act
            client.UpdateApiKey();

            // Assert
            oldApiKey.Should().NotBeEquivalentTo(client.ApiKey);
        }

        [Fact]
        public void UpdateSecretKey_WhenUpdateSecretKey_ShouldChange()
        {
            // Arrange
            var (client, oldRawSecretKey) = Factory.ApiClient.GenerateApiClient();
            var oldSecretKey = client.SecretKey;

            // Act
            var newRawSecretKey = client.UpdateSecretKey();

            // Assert
            oldRawSecretKey.Should().NotBeEquivalentTo(newRawSecretKey);
            oldSecretKey.Should().NotBeEquivalentTo(client.SecretKey);
        }
    }
}
