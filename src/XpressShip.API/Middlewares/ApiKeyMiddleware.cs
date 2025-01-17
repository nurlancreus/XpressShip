using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using XpressShip.API.Attributes;
using XpressShip.Infrastructure.Persistence;

namespace XpressShip.API.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            // Check if the endpoint has the AllowAnonymousApiKey attribute
            var endpoint = context.GetEndpoint();
            var authorizeClient = endpoint?.Metadata.GetMetadata<AuthorizeApiClientAttribute>() != null;

            if (!authorizeClient)
            {
                await _next(context); // Bypass authentication
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKeyValues) ||
                !context.Request.Headers.TryGetValue("X-Secret-Key", out var extractedSecretKeyValues))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key or Secret Key is missing.");
                return;
            }

            var apiKey = extractedApiKeyValues.FirstOrDefault();
            var secretKey = extractedSecretKeyValues.FirstOrDefault();

            var client = await dbContext.ApiClients
                .FirstOrDefaultAsync(a => a.ApiKey == apiKey && a.IsActive);

            if (client == null || string.IsNullOrEmpty(secretKey) || !VerifySecretKey(secretKey!, client.SecretKey))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Invalid API Key or Secret Key.");
                return;
            }

            await _next(context);
        }

        private static bool VerifySecretKey(string providedSecret, string storedSecret)
        {
            // Use HMACSHA256 for secret key comparison
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(storedSecret));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(providedSecret));
            return storedSecret == Convert.ToBase64String(computedHash);
        }
    }
}
