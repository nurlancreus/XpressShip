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
        public async Task<Result<TokenDTO>> CreateAccessTokenAsync(ApplicationUser user)
        {
            var lifeTime = _accessSettings.AccessTokenLifeTimeInMinutes;

            TokenDTO token = new()
            {
                ExpiresAt = DateTime.UtcNow.AddMinutes(lifeTime),
            };

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_accessSettings.SecurityKey));

            // Create the encrypted credentials.
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.UserName!), // Name claim (username)
                new(ClaimTypes.GivenName, user.FirstName!),
                new(ClaimTypes.Surname, user.LastName!),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject (user id)
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT unique ID (JTI)
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()), // Issued at (Unix timestamp)
                new(ClaimTypes.NameIdentifier, user.Id), // Unique name identifier of the user (id)
                new(ClaimTypes.Email, user.Email!) // Email of the user
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Set the token's configurations.
            JwtSecurityToken securityToken = new(
                audience: _accessSettings.Audience,
                issuer: _accessSettings.Issuer,
                expires: token.ExpiresAt,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials,
                claims: claims
            );

            // Create an instance of the token handler class.
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken = tokenHandler.WriteToken(securityToken);

            // Generate the refresh token.
            var refreshTokenResult = await CreateRefreshTokenAsync();

            if (!refreshTokenResult.IsSuccess) return Result<TokenDTO>.Failure(Error.UnexpectedError("Could not create refresh token."));

            token.RefreshToken = refreshTokenResult.Value;

            return Result<TokenDTO>.Success(token);
        }

        public async Task<Result<string>> CreateRefreshTokenAsync()
        {
            var token = await Task.Run(() =>
            {
                byte[] number = new byte[64];

                using RandomNumberGenerator random = RandomNumberGenerator.Create();
                random.GetBytes(number);

                return Convert.ToBase64String(number);
            });

            return Result<string>.Success(token);
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
    }
}
