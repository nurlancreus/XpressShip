using Azure.Core;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Services.Token;
using XpressShip.Application.DTOs.Token;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.Auth.Common.RefreshLogin
{
    public class RefreshLoginHandler : ICommandHandler<RefreshLoginCommand, TokenDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public RefreshLoginHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<TokenDTO>> Handle(RefreshLoginCommand request, CancellationToken cancellationToken)
        {
            var principalResult = _tokenService.GetPrincipalFromAccessToken(request.AccessToken);

            if (!principalResult.IsSuccess) return Result<TokenDTO>.Failure(principalResult.Error);

            string? name = principalResult.Value.FindFirstValue(ClaimTypes.Name);

            ApplicationUser? user = null;

            if (name is not null) await _userManager.FindByNameAsync(name);

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenEndDate <= DateTime.UtcNow)
            {
                return Result<TokenDTO>.Failure(Error.UnauthorizedError("Invalid refresh token"));
            }

            var tokenResult = await _tokenService.GetTokenAsync(user);

            if (!tokenResult.IsSuccess) return Result<TokenDTO>.Failure(tokenResult.Error);

            var token = tokenResult.Value;

            await _tokenService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.ExpiresAt);

            return Result<TokenDTO>.Success(token);
        }
    }
}
