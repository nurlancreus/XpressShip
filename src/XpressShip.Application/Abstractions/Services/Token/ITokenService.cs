using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.DTOs.Token;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Abstractions.Services.Token
{
    public interface ITokenService
    {
        Task UpdateRefreshTokenAsync(string refreshToken, ApplicationUser user, DateTime accessTokenEndDate);
        Task<Result<TokenDTO>> GetTokenAsync(ApplicationUser user);
        Task<Result<string>> CreateAccessTokenAsync(ApplicationUser user);
        Result<string> CreateRefreshToken();
        Result<ClaimsPrincipal> GetPrincipalFromAccessToken(string? accessToken);
    }
}
