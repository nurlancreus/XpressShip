using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Infrastructure.Services.Session
{
    public class JwtSession(IHttpContextAccessor httpContextAccessor) : IJwtSession
    {
        private readonly ClaimsPrincipal? _claimsPrincipal = httpContextAccessor?.HttpContext?.User;

        public Result<IEnumerable<string>> GetRoles()
        {
            var isUserAuthResult = IsUserAuth();

            if (!isUserAuthResult.IsSuccess) return Result<IEnumerable<string>>.Failure(isUserAuthResult.Error);

            var roleNames = _claimsPrincipal!.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

            return Result<IEnumerable<string>>.Success(roleNames);
        }

        public Result<string> GetUserEmail()
        {
            var isUserAuthResult = IsUserAuth();

            if (!isUserAuthResult.IsSuccess) return Result<string>.Failure(isUserAuthResult.Error);

            var emailClaim = _claimsPrincipal!.FindFirst(ClaimTypes.Email)?.Value;

            if (emailClaim is null) return Result<string>.Failure(Error.UnauthorizedError("Email claim is missing in the claim"));

            return Result<string>.Success(emailClaim);
        }

        public Result<string> GetUserId()
        {
            var isUserAuthResult = IsUserAuth();

            if (!isUserAuthResult.IsSuccess) return Result<string>.Failure(isUserAuthResult.Error);

            var idClaim = _claimsPrincipal!.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (idClaim is null) return Result<string>.Failure(Error.UnauthorizedError("Id claim is missing in the claim"));

            return Result<string>.Success(idClaim);
        }

        public Result<string> GetUserName()
        {
            var isUserAuthResult = IsUserAuth();

            if (!isUserAuthResult.IsSuccess) return Result<string>.Failure(isUserAuthResult.Error);

            var nameClaim = _claimsPrincipal!.FindFirst(ClaimTypes.Name)?.Value;

            if (nameClaim is null) return Result<string>.Failure(Error.UnauthorizedError("Username claim is missing in the claim"));

            return Result<string>.Success(nameClaim);
        }

        public Result<bool> IsAdminAuth()
        {
            var userRolesResult = GetRoles();

            if (!userRolesResult.IsSuccess) return Result<bool>.Failure(userRolesResult.Error);

            var userRoles = userRolesResult.Value;

            var isAdminOrSuperAdminDefined = userRoles.Contains("Admin") || userRoles.Contains("SuperAdmin");

            if (!isAdminOrSuperAdminDefined) return Result<bool>.Failure(Error.UnauthorizedError("User is not authorized"));

            return Result<bool>.Success(true);
        }

        public Result<bool> IsRolesAuth(IEnumerable<string> roleNames)
        {
            var userRolesResult = GetRoles();

            if (!userRolesResult.IsSuccess) return Result<bool>.Failure(userRolesResult.Error);

            var userRoles = userRolesResult.Value;

            var isAnyRoleDefined = roleNames.Any(userRoles.Contains);

            if (!isAnyRoleDefined) return Result<bool>.Failure(Error.UnauthorizedError("User is not authorized"));

            return Result<bool>.Success(true);
        }

        public Result<bool> IsSuperAdminAuth()
        {
            var userRolesResult = GetRoles();

            if (!userRolesResult.IsSuccess) return Result<bool>.Failure(userRolesResult.Error);

            var userRoles = userRolesResult.Value;

            var isSuperAdminDefined = userRoles.Contains("SuperAdmin");

            if (!isSuperAdminDefined) return Result<bool>.Failure(Error.UnauthorizedError("User is not authorized"));

            return Result<bool>.Success(true);
        }

        public Result<bool> IsUserAuth()
        {
            if (!(_claimsPrincipal?.Identity?.IsAuthenticated ?? false)) return Result<bool>.Failure(Error.UnauthorizedError("User is not authorized"));

            return Result<bool>.Success(true);
        }
    }
}
