using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Domain.Extensions;

namespace XpressShip.Infrastructure.Services.Session
{
    public class SessionService(IHttpContextAccessor httpContextAccessor) : ISessionService
    {
        private readonly IHeaderDictionary? _headers = httpContextAccessor?.HttpContext?.Request.Headers;

        public string GetApiKey()
        {
            // Extract API Key values from request headers
            if (_headers == null || !_headers.TryGetValue("X-Api-Key", out var apiKeyValues))
            {
                throw new ValidationException("API Key is missing.");
            }

            var extractedApiKey = apiKeyValues.FirstOrDefault();

            return extractedApiKey.EnsureNonNull("API Key");
        }

        public (string apiKey, string secretKey) GetClientApiAndSecretKey()
        {
            // Extract API Key and Secret Key values from request headers
            if (_headers == null || !_headers.TryGetValue("X-Api-Key", out var apiKeyValues) ||
                !_headers.TryGetValue("X-Secret-Key", out var secretKeyValues))
            {
                throw new ValidationException("API Key or Secret Key is missing.");
            }

            var extractedApiKey = apiKeyValues.FirstOrDefault();
            var extractedSecretKey = secretKeyValues.FirstOrDefault();

            var apiKey = extractedApiKey.EnsureNonNull("API Key");
            var secretKey = extractedSecretKey.EnsureNonNull("Secret Key");

            return (apiKey, secretKey);
        }

        public string GetSecretKey()
        {
            // Extract Secret Key values from request headers
            if (_headers == null || !_headers.TryGetValue("X-Secret-Key", out var secretKeyValues))
            {
                throw new ValidationException("Secret Key is missing.");
            }

            var extractedSecretKey = secretKeyValues.FirstOrDefault();

            return extractedSecretKey.EnsureNonNull("Secret Key");
        }
    }
}
