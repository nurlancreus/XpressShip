using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Abstractions.Services.Session
{
    public interface IJwtSession
    {
        Result<bool> IsUserAuth();
        Result<bool> IsUserActive();
        Result<bool> IsSuperAdminAuth();
        Result<bool> IsAdminAuth();
        Result<bool> IsRolesAuth(IEnumerable<string> roleNames);
        Result<IEnumerable<string>> GetRoles();
        Result<string> GetUserId();
        Result<string> GetUserName();
        Result<string> GetUserEmail();

    }
}
