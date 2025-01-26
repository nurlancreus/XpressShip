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
        public static string GetUserGroupName(string identifier, InitiatorType userType)
        {
            return userType switch
            {
                InitiatorType.ApiClient => $"{GroupNames.ApiClientGroup}_{identifier}",
                InitiatorType.Account => $"{GroupNames.AccountGroup}_{identifier}",
                _ => $"{GroupNames.ApiClientGroup}_{identifier}"
            };
        }
    }
}
