using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Domain.Entities;

namespace XpressShip.API.Handlers
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IApiClientRepository _apiClientRepository;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IApiClientRepository apiClientRepository)
            : base(options, logger, encoder)
        {
            _apiClientRepository = apiClientRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Api-Key", out var apiKeyValues) ||
                !Request.Headers.TryGetValue("X-Secret-Key", out var secretKeyValues))
            {
                return AuthenticateResult.NoResult(); // No credentials provided, skip authentication
            }

            var apiKey = apiKeyValues.FirstOrDefault();
            var providedSecretKey = secretKeyValues.FirstOrDefault();

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(providedSecretKey))
            {
                return AuthenticateResult.Fail("API Key or Secret Key is missing.");
            }

            var apiClient = await _apiClientRepository.Table
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ApiKey == apiKey && c.IsActive);

            if (apiClient == null || !ApiClient.VerifySecretKey(providedSecretKey, apiClient.SecretKey))
            {
                return AuthenticateResult.Fail("Invalid API Key or Secret Key.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, apiClient.CompanyName),
                new Claim(ClaimTypes.Role, "ApiClient"),
                new Claim("ApiClientId", apiClient.Id.ToString()) // Store API Client ID
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
