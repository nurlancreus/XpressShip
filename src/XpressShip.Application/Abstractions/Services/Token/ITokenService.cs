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
        Task<Result<TokenDTO>> CreateAccessTokenAsync(ApplicationUser user);
        Task<Result<string>> CreateRefreshTokenAsync();
        Result<ClaimsPrincipal> GetPrincipalFromAccessToken(string? accessToken);
    }
}
