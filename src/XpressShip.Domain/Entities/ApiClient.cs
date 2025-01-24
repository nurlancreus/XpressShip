using System.Security.Cryptography;
using System.Text;
using XpressShip.Domain;
using XpressShip.Domain.Entities.Base;
using XpressShip.Domain.Extensions;

namespace XpressShip.Domain.Entities
{
    public class ApiClient : BaseEntity
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? DeActivatedAt { get; set; }
        public Address Address { get; set; } = null!;

        public ICollection<Shipment> Shipments { get; set; } = [];

        private ApiClient() { }
        private ApiClient(string companyName, string email)
        {
            CompanyName = companyName;
            Email = email;
            ApiKey = GenerateApiKey();
            SecretKey = GenerateSecretKey();
            IsActive = true;
        }

        public static ApiClient Create(string companyName, string email)
        {
            companyName.EnsureNonEmpty();
            email.EnsureNonEmpty();

            email.EnsureValidEmail();

            return new ApiClient(companyName, email);
        }

        public void Toggle()
        {
            if (IsActive)
            {
                IsActive = false;
                DeActivatedAt = DateTime.UtcNow;
            }
            else
            {
                IsActive = true;
                DeActivatedAt = null;
            }
        }

        public void UpdateApiKey() => ApiKey = GenerateApiKey();
        public void UpdaterSecretKey() => ApiKey = GenerateSecretKey();

        private static string GenerateApiKey()
        {
            var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            return HashKey(key);
        }

        private static string GenerateSecretKey()
        {
            var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return HashKey(key);
        }
        private static string HashKey(string key)
        {
            using var hmac = new HMACSHA256();
            var hashedKey = hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
            return Convert.ToBase64String(hashedKey);
        }
    }
}
