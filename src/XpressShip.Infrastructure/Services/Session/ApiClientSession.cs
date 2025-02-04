using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Extensions;

namespace XpressShip.Infrastructure.Services.Session
{
    public class ApiClientSession(IHttpContextAccessor httpContextAccessor) : IApiClientSession
    {
        private readonly IHeaderDictionary? _headers = httpContextAccessor?.HttpContext?.Request.Headers;
        private readonly ClaimsPrincipal? _claimsPrincipal = httpContextAccessor?.HttpContext?.User;

        public Result<string> GetApiKey()
        {
            if (_headers != null && _headers.TryGetValue("X-Api-Key", out var apiKeyValues) && apiKeyValues.FirstOrDefault() is string apiKey) return Result<string>.Success(apiKey);

            return Result<string>.Failure(Error.BadRequestError("API Key is missing."));
        }

        public Result<(string apiKey, string secretKey)> GetClientApiAndSecretKey()
        {
            if (_headers != null && _headers.TryGetValue("X-Secret-Key", out var secretKeyValues) && _headers.TryGetValue("X-Api-Key", out var apiKeyValues) && apiKeyValues.FirstOrDefault() is string apiKey && secretKeyValues.FirstOrDefault() is string secretKey)

                return Result<(string apiKey, string secretKey)>.Success((apiKey, secretKey));

            return Result<(string apiKey, string secretKey)>.Failure(Error.BadRequestError("API Key or Secret Key is missing."));
        }

        public Result<Guid> GetClientId()
        {
            if (!(_claimsPrincipal?.Identity?.IsAuthenticated ?? false))
                return Result<Guid>.Failure(Error.UnauthorizedError("Client is not authorized"));

            var clientId = _claimsPrincipal.FindFirstValue("ApiClientId");

            if (clientId == null) return Result<Guid>.Failure(Error.UnauthorizedError("ApiClientId claim is missing in the claim"));

            return Result<Guid>.Success(Guid.Parse(clientId));
        }

        public Result<string> GetSecretKey()
        {
            if (_headers != null && _headers.TryGetValue("X-Secret-Key", out var secretKeyValues) && secretKeyValues.FirstOrDefault() is string secretKey) return Result<string>.Success(secretKey);

            return Result<string>.Failure(Error.BadRequestError("Secret Key is missing."));
        }
    }
}
