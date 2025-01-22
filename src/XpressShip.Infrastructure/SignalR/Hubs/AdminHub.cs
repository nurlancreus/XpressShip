using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Infrastructure.SignalR.Constants;

namespace XpressShip.Infrastructure.SignalR.Hubs
{
    public class AdminHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;

            if (user?.IsInRole("Admin") ?? false)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.AdminGroup);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupNames.AdminGroup);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
