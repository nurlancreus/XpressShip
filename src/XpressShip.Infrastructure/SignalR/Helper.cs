using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Enums;
using XpressShip.Infrastructure.SignalR.Constants;

namespace XpressShip.Infrastructure.SignalR
{
    public static class Helper
    {
        public static string GetUserGroupName(string identifier, UserType userType)
        {
            return userType switch
            {
                UserType.ApiClient => $"{GroupNames.ApiClientGroup}_{identifier}",
                UserType.Account => $"{GroupNames.AccountGroup}_{identifier}",
                _ => $"{GroupNames.ApiClientGroup}_{identifier}"
            };
        }
    }
}
