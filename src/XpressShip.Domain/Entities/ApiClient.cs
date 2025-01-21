using System.Security.Cryptography;
using XpressShip.Domain;
using XpressShip.Domain.Entities.Base;

namespace XpressShip.Domain.Entities
{
    public class ApiClient : BaseEntity
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;  // Public API Key
        public string SecretKey { get; set; } = string.Empty;  // Secret Key for HMAC
        public bool IsActive { get; set; } = true;
        public Address Address { get; set; } = null!;  // Navigation Property to Address

        public ICollection<Shipment> Shipments { get; set; } = []; // Navigation Property to Shipments

        private ApiClient()
        {

        }
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
            return new ApiClient(companyName, email);
        }

        public void Toggle()
        {
            if (IsActive) IsActive = false;
            else IsActive = true;
        }

        public void UpdateApiKey() => ApiKey = GenerateApiKey();
        public void UpdaterSecretKey() => ApiKey = GenerateSecretKey();

        private static string GenerateApiKey()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));  // 256-bit key
        }

        private static string GenerateSecretKey()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));  // 512-bit key
        }
    }
}
