using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using XpressShip.Domain;
using XpressShip.Domain.Entities.Base;
using XpressShip.Domain.Extensions;

namespace XpressShip.Domain.Entities
{
    public class ApiClient : BaseEntity
    {
        [NotMapped]
        private static readonly string AppSecretKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(12));

        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? DeActivatedAt { get; set; }
        public Address Address { get; set; } = null!;

        public ICollection<Shipment> Shipments { get; set; } = [];

        private ApiClient() { }
        private ApiClient(string companyName, string email, string hashedSecretKey)
        {
            CompanyName = companyName;
            Email = email;
            ApiKey = GenerateApiKey();
            SecretKey = hashedSecretKey;
            IsActive = false;
        }

        public static (ApiClient client, string rawSecretKey) Create(string companyName, string email)
        {
            companyName.EnsureNonEmpty();
            email.EnsureNonEmpty();

            email.EnsureValidEmail();

            var rawSecretKey = GenerateSecretKey();
            var hashedSecretKey = HashKey(rawSecretKey);

            var client = new ApiClient(companyName, email, hashedSecretKey);

            return (client, rawSecretKey);
        }

        public void Toggle()
        {
            IsActive = !IsActive;

            DeActivatedAt = !IsActive ? DateTime.UtcNow : null;
        }

        public string UpdateApiKey()
        {
            var newApiKey = GenerateApiKey();
            ApiKey = newApiKey;

            return newApiKey;
        }
        public string UpdateSecretKey()
        {
            var newRawSecretKey = GenerateSecretKey();
            SecretKey = HashKey(newRawSecretKey);

            return newRawSecretKey;
        }

        private static string GenerateApiKey() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        private static string GenerateSecretKey() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private static string HashKey(string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(AppSecretKey));
            var hashedKey = hmac.ComputeHash(Encoding.UTF8.GetBytes(key));

            return Convert.ToBase64String(hashedKey);
        }

        public static bool VerifySecretKey(string providedSecret, string storedHashedSecret)
        {
            var computedBase64Hash = HashKey(providedSecret);

            return storedHashedSecret == computedBase64Hash;
        }
    }
}
