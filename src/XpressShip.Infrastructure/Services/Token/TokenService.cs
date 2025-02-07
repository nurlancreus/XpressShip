using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Services.Token;
using XpressShip.Application.DTOs.Token;
using XpressShip.Application.Options.Token;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Infrastructure.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly AccessSettings _accessSettings;
        private readonly RefreshSettings _refreshSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IOptions<TokenSettings> options, UserManager<ApplicationUser> userManager)
        {
            _accessSettings = options.Value.Access;
            _refreshSettings = options.Value.Refresh;
            _userManager = userManager;
        }
        public async Task<Result<string>> CreateAccessTokenAsync(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessSettings.SecurityKey));

            // Create the encrypted credentials.
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.UserName!), // Name claim (username)
                new(ClaimTypes.GivenName, user.FirstName!),
                new(ClaimTypes.Surname, user.LastName!),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject (user id)
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT unique ID (JTI)
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()), // Issued at (Unix timestamp)
                new(ClaimTypes.NameIdentifier, user.Id), // Unique name identifier of the user (id)
                new(ClaimTypes.Email, user.Email!), // Email of the user
                new("IsActive", user.IsActive.ToString()) // Include the IsActive field as a custom claim
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Set the token's configurations.
            var securityToken = new JwtSecurityToken(
                audience: _accessSettings.Audience,
                issuer: _accessSettings.Issuer,
                expires: DateTime.UtcNow.AddMinutes(_accessSettings.AccessTokenLifeTimeInMinutes),
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials,
                claims: claims
            );

            // Create an instance of the token handler class.
            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.WriteToken(securityToken);

            return Result<string>.Success(accessToken);
        }

        public Result<string> CreateRefreshToken()
        {
            byte[] number = new byte[64];

            using RandomNumberGenerator random = RandomNumberGenerator.Create();
            random.GetBytes(number);

            var token = Convert.ToBase64String(number);

            return Result<string>.Success(token);
        }

        public async Task<Result<TokenDTO>> GetTokenAsync(ApplicationUser user)
        {
            var token = new TokenDTO();

            // Create the access token
            var accessTokenResult = await CreateAccessTokenAsync(user);

            if (!accessTokenResult.IsSuccess) return Result<TokenDTO>.Failure(Error.TokenError("Could not create access token."));

            var accessToken = accessTokenResult.Value;
            var accessTokenEndDate = DateTime.UtcNow.AddMinutes(_accessSettings.AccessTokenLifeTimeInMinutes);

            // Create the refresh token
            var refreshTokenResult = CreateRefreshToken();

            if (!refreshTokenResult.IsSuccess) return Result<TokenDTO>.Failure(Error.TokenError("Could not create refresh token."));

            var refreshToken = refreshTokenResult.Value;

            token.AccessToken = accessToken;
            token.ExpiresAt = accessTokenEndDate;
            token.RefreshToken = refreshToken;

            return Result<TokenDTO>.Success(token);
        }

        public Result<ClaimsPrincipal> GetPrincipalFromAccessToken(string? accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = _accessSettings.Audience,
                ValidateIssuer = true,
                ValidIssuer = _accessSettings.Issuer,
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessSettings.SecurityKey)),

                ValidateLifetime = false //should be false
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Result<ClaimsPrincipal>.Failure(Error.ValidationError("Invalid token"));
            }

            return Result<ClaimsPrincipal>.Success(principal);
        }

        public async Task UpdateRefreshTokenAsync(string refreshToken, ApplicationUser user, DateTime accessTokenEndDate)
        {
            var refreshTokenEndDate = accessTokenEndDate.AddMinutes(_refreshSettings.RefreshTokenLifeTimeInMinutes);

            user.RefreshToken = refreshToken;
            user.RefreshTokenEndDate = refreshTokenEndDate;

            await _userManager.UpdateAsync(user);
        }
    }
}
