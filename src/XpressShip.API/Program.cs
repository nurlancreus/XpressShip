using XpressShip.API.Endpoints;

namespace XpressShip.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.RegisterServices();

            var app = builder.Build();

            app.RegisterMiddlewares();

            app.RegisterAuthEndpoints()
               .RegisterApiClientEndpoints()
               .RegisterShipmentEndpoints()
               .RegisterPaymentEndpoints()
               .RegisterWebHookEndpoints();

            app.Run();
        }
    }
}
