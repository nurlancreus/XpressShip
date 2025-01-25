using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Infrastructure.Services.Session
{
    // FIND A WAY TO INVALIDATE TOKEN WHEN CLAIMS HAVE CHANGED
    public class JwtSession(IHttpContextAccessor httpContextAccessor) : IJwtSession
    {
        private readonly ClaimsPrincipal? _claimsPrincipal = httpContextAccessor?.HttpContext?.User;

        private enum AppClaimType : byte
        {
            Id,
            Email,
            UserName,
            IsActive
        }

        public Result<IEnumerable<string>> GetRoles()
        {
            if (_claimsPrincipal == null)
                return Result<IEnumerable<string>>.Failure(Error.UnauthorizedError("User is not authorized"));

            var isUserAuthResult = IsUserAuth();

            if (!isUserAuthResult.IsSuccess) return Result<IEnumerable<string>>.Failure(isUserAuthResult.Error);

            var isUserActiveResult = IsUserActive();

            if (!isUserActiveResult.IsSuccess) return Result<IEnumerable<string>>.Failure(isUserActiveResult.Error);

            var roleNames = _claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

            return Result<IEnumerable<string>>.Success(roleNames);
        }

        public Result<string> GetUserEmail() => GetClaim(AppClaimType.Email);

        public Result<string> GetUserId() => GetClaim(AppClaimType.Id);

        public Result<string> GetUserName() => GetClaim(AppClaimType.UserName);

        public Result<bool> IsSuperAdminAuth() => IsRolesAuth(["SuperAdmin"]);

        public Result<bool> IsAdminAuth() => IsRolesAuth(["SuperAdmin", "Admin"]);

        public Result<bool> IsRolesAuth(IEnumerable<string> roleNames)
        {
            var userRolesResult = GetRoles();

            if (!userRolesResult.IsSuccess) return Result<bool>.Failure(userRolesResult.Error);

            var userRoles = userRolesResult.Value;

            var isAnyRoleDefined = roleNames.Any(userRoles.Contains);

            if (!isAnyRoleDefined) return Result<bool>.Failure(Error.UnauthorizedError("User is not authorized"));

            return Result<bool>.Success(true);
        }

        public Result<bool> IsUserAuth()
        {
            if (!(_claimsPrincipal?.Identity?.IsAuthenticated ?? false))
                return Result<bool>.Failure(Error.UnauthorizedError("User is not authorized"));

            return Result<bool>.Success(true);
        }

        public Result<bool> IsUserActive()
        {
            var claimResult = GetClaim(AppClaimType.IsActive);

            if (!claimResult.IsSuccess) return Result<bool>.Failure(claimResult.Error);

            if (!bool.TryParse(claimResult.Value, out bool isActive))
                return Result<bool>.Failure(Error.BadRequestError("Invalid format for IsActive claim"));

            if (!isActive)
                return Result<bool>.Failure(Error.UnauthorizedError("User is not authorized"));

            return Result<bool>.Success(true);
        }

        private Result<string> GetClaim(AppClaimType claimType)
        {
            if (_claimsPrincipal == null)
                return Result<string>.Failure(Error.UnauthorizedError("User is not authorized"));

            if (!IsUserAuth().IsSuccess)
                return Result<string>.Failure(Error.UnauthorizedError("User is not authorized"));

            string? claimValue, errorMessage;

            switch (claimType)
            {
                case AppClaimType.Id:
                    claimValue = _claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                    errorMessage = "Id claim is missing in the claim";
                    break;

                case AppClaimType.Email:
                    claimValue = _claimsPrincipal.FindFirstValue(ClaimTypes.Email);
                    errorMessage = "Email claim is missing in the claim";
                    break;

                case AppClaimType.UserName:
                    claimValue = _claimsPrincipal.FindFirstValue(ClaimTypes.Name);
                    errorMessage = "Name claim is missing in the claim";
                    break;

                case AppClaimType.IsActive:
                    claimValue = _claimsPrincipal.FindFirstValue("IsActive");
                    errorMessage = "IsActive claim is missing in the claim";
                    break;

                default:
                    return Result<string>.Failure(Error.BadRequestError("Claim type is not valid"));
            }

            if (claimValue == null)
                return Result<string>.Failure(Error.UnauthorizedError(errorMessage));

            return Result<string>.Success(claimValue);
        }
    }
}
