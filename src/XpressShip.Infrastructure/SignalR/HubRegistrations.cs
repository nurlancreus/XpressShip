using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Infrastructure.SignalR.Hubs;
using AdminHub = XpressShip.Infrastructure.SignalR.Hubs.AdminHub;
using PaymentHub = XpressShip.Infrastructure.SignalR.Hubs.PaymentHub;
using ShipmentHub = XpressShip.Infrastructure.SignalR.Hubs.ShipmentHub;

namespace XpressShip.Infrastructure.SignalR
{
    public static class HubRegistrations
    {
        public static void MapHubs(this WebApplication app)
        {
            app.MapHub<PaymentHub>("/hubs/payment");
            app.MapHub<ShipmentHub>("/hubs/shipment");
            app.MapHub<AdminHub>("/hubs/admin");
        }
    }
}
