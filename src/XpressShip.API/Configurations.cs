using Microsoft.EntityFrameworkCore;
using XpressShip.API.Middlewares;
using XpressShip.Application.Behaviours;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services;
using XpressShip.Application.Interfaces.Services.Mail;
using XpressShip.Application.Interfaces.Services.Mail.Template;
using XpressShip.Application.Interfaces.Services.Payment;
using XpressShip.Application.Interfaces.Services.Payment.Stripe;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Options;
using XpressShip.Application.Options.PaymentGateway;
using XpressShip.Domain.Validation;
using XpressShip.Infrastructure.Persistence;
using XpressShip.Infrastructure.Persistence.Repositories;
using XpressShip.Infrastructure.Services;
using XpressShip.Infrastructure.Services.Mail;
using XpressShip.Infrastructure.Services.Mail.Template;
using XpressShip.Infrastructure.Services.Payment;
using XpressShip.Infrastructure.Services.Payment.Stripe;
using XpressShip.Infrastructure.Services.Session;
using XpressShip.Infrastructure.Services.Validation;
using XpressShip.Infrastructure.SignalR;

namespace XpressShip.API
{
    public static class Configurations
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();

            // Add ProblemDetails services
            builder.Services.AddProblemDetails();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Configure DbContext with Scoped lifetime
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Mock"), sqlOptions => sqlOptions
                .MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
                .EnableSensitiveDataLogging();
            });

            // Register SignalR
            builder.Services.RegisterSignalRServices();

            // Register MediatR
            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(ValidationPipelineBehaviour<,>).Assembly);
                config.AddOpenBehavior(typeof(ValidationPipelineBehaviour<,>));
                config.AddOpenBehavior(typeof(LoggingPipelineBehaviour<,>));

            });

            // Register Repositories
            #region Register Repositories
            builder.Services.AddScoped<IApiClientRepository, ApiClientRepository>();
            builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IShipmentRateRepository, ShipmentRateRepository>();
            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            // Register Services
            #region Register Client Services
            builder.Services.AddScoped<IApiClientSessionService, SessionService>();
            builder.Services.AddScoped<IAddressValidationService, AddressValidationService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IGeoInfoService, GeoInfoService>();

            builder.Services.AddScoped<IDistanceService, DistanceService>();

            builder.Services.AddScoped<IPaymentMailTemplatesService, PaymentMailTemplatesService>();
            builder.Services.AddScoped<IShipmentMailTemplatesService, ShipmentMailTemplatesService>();

            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IStripeService, StripeService>();
            #endregion


            // Register Options pattern
            builder.Services.Configure<APISettings>(APISettings.GeoCodeAPI,
                    builder.Configuration.GetSection("API:GeoCodeAPI"));

            builder.Services.Configure<ShippingRatesSettings>(builder.Configuration.GetSection("ShippingRatesSettings"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailConfiguration"));
            builder.Services.Configure<PaymentGatewaySettings>(builder.Configuration.GetSection("PaymentGateways"));
        }

        public static void RegisterMiddlewares(this WebApplication app)
        {
            app.UseExceptionHandler();

            app.UseStatusCodePages(async statusCodeCntx
                    => await TypedResults.Problem(statusCode: statusCodeCntx.HttpContext.Response.StatusCode)
                 .ExecuteAsync(statusCodeCntx.HttpContext));


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<ApiKeyMiddleware>();

            app.MapHubs();
        }
    }

}
