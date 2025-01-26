using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Enums;
using XpressShip.Infrastructure.SignalR.Constants;

namespace XpressShip.Infrastructure.SignalR.Hubs
{
    public class PaymentHub : Hub
    {
        // Add the client to their group based on ApiKey or Identity UserId
        public async Task JoinGroup(string? apiKey = null)
        {
            if ((Context.User?.Identity?.IsAuthenticated ?? false) && Context.UserIdentifier is string userId)
            {

                await Groups.AddToGroupAsync(Context.ConnectionId, Helper.GetUserGroupName(userId, InitiatorType.Account));

            }
            else if (!string.IsNullOrEmpty(apiKey))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Helper.GetUserGroupName(apiKey, InitiatorType.ApiClient));
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.AdminGroup);
        }

        // Remove the client from their group when they disconnect
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Helper.GetUserGroupName(userId, InitiatorType.Account));
            }

            var apiKey = Context.GetHttpContext()?.Request.Headers["X-Api-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(apiKey))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Helper.GetUserGroupName(apiKey, InitiatorType.ApiClient));
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupNames.AdminGroup);

            await base.OnDisconnectedAsync(exception);
        }
    }

}
