using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Domain
{
    public class Generator
    {
        public static string GenerateApiKey()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));  // 256-bit key
        }

        public static string GenerateSecretKey()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));  // 512-bit key
        }

        public static string GenerateTrackingNumber()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var uniquePart = new string(Enumerable.Repeat(chars, 8)
                                                  .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"TRK-{DateTime.UtcNow:yyyyMMdd}-{uniquePart}";  // Example: TRK-20240101-AB12CD34
        }
    }
}
