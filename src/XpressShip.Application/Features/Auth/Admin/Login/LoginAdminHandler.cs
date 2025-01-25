using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Services.Token;
using XpressShip.Application.DTOs.Token;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.Auth.Admin.Login
{
    public class LoginAdminHandler : ICommandHandler<LoginAdminCommand, TokenDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public LoginAdminHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<Result<TokenDTO>> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
        {
            var admin = await _userManager.FindByNameAsync(request.UserName);

            if (admin is null) return Result<TokenDTO>.Failure(Error.NotFoundError(nameof(admin)));

            var signInResult = await _signInManager.PasswordSignInAsync(admin, request.Password, false, false);

            if (!signInResult.Succeeded) return Result<TokenDTO>.Failure(Error.LoginError());

            var tokenResult = await _tokenService.GetTokenAsync(admin);

            if (!tokenResult.IsSuccess) return Result<TokenDTO>.Failure(tokenResult.Error);

            var token = tokenResult.Value;

            await _tokenService.UpdateRefreshTokenAsync(token.RefreshToken, admin, token.ExpiresAt);

            return Result<TokenDTO>.Success(token);
        }
    }
}
