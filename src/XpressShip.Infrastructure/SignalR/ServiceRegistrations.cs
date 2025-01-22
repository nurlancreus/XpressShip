using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Hubs;
using XpressShip.Infrastructure.SignalR.HubServices;

namespace XpressShip.Infrastructure.SignalR
{
    public static class ServiceRegistratios
    {
        public static IServiceCollection RegisterSignalRServices(this IServiceCollection services)
        {
            services.AddTransient<IPaymentHubService, PaymentHubService>();
            services.AddTransient<IShipmentHubService, ShipmentHubService>();
            services.AddTransient<IAdminHubService, AdminHubService>();

            services.AddSignalR();

            return services;
        }
    }
}
