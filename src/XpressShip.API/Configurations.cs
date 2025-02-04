using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Abstractions.Services.Mail;
using XpressShip.Application.Abstractions.Services.Mail.Template;
using XpressShip.Application.Abstractions.Services.Payment;
using XpressShip.Application.Abstractions.Services.Payment.Stripe;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Behaviours;
using XpressShip.Application.Options;
using XpressShip.Application.Options.PaymentGateway;
using XpressShip.Application.Options.Token;
using XpressShip.Domain.Entities.Identity;
using XpressShip.Domain.Entities.Users;
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
using XpressShip.Application.Features.ApiClients.Commands.Create;
using Microsoft.AspNetCore.Authentication;
using XpressShip.API.Handlers;

namespace XpressShip.API
{
    public static class Configurations
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            #region Register Identity
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });
            #endregion

            #region Register Auth
            // Configure Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters()
                 {
                     ValidateAudience = true,
                     ValidAudience = builder.Configuration["Token:Access:Audience"],
                     ValidateIssuer = true,
                     ValidIssuer = builder.Configuration["Token:Access:Issuer"],
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     
                     IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Token:Access:SecurityKey"]!)),

                     LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null && expires > DateTime.UtcNow,

                     NameClaimType = ClaimTypes.Name,
                     RoleClaimType = ClaimTypes.Role
                 };
             })
             .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

            // Configure Authorization Policies
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy(Constants.AdminsPolicy, policy => policy.RequireAuthenticatedUser().RequireRole("Admin", "SuperAdmin"))
                .AddPolicy(Constants.RegisteredUsersPolicy,policy => policy.RequireAuthenticatedUser().RequireRole("Sender", "Admin", "SuperAdmin"))
                .AddPolicy(Constants.ApiClientsPolicy, policy => policy.RequireAuthenticatedUser().RequireRole("ApiClient"))
                .AddPolicy(Constants.AdminsOrApiClientsPolicy, policy => policy.RequireAssertion(context => 
                   context.User.IsInRole("Admin")
                || context.User.IsInRole("SuperAdmin")
                || context.User.IsInRole("ApiClient")))
                .AddPolicy(Constants.SendersOrApiClientsPolicy, policy => policy.RequireAssertion(context =>
                   context.User.IsInRole("Sender")
                || context.User.IsInRole("ApiClient")))
                .AddPolicy(Constants.RegisteredUsersOrApiClientsPolicy, policy => policy.RequireAssertion(context => context.User.IsInRole("Sender") 
                || context.User.IsInRole("Admin") 
                || context.User.IsInRole("SuperAdmin") 
                || context.User.IsInRole("ApiClient")));
            #endregion

            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();

            // Add Memory Cache
            builder.Services.AddMemoryCache();

            // Add Exception Handler
            builder.Services.AddExceptionHandler<CustomExceptionHandler>();

            // Add ProblemDetails services
            builder.Services.AddProblemDetails();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Configure DbContext with Scoped lifetime
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), sqlOptions => sqlOptions
                .MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
                .EnableSensitiveDataLogging();
            });

            // Register Fluent Validation
            builder.Services.AddFluentValidationClientsideAdapters();

            builder.Services
                .AddValidatorsFromAssemblyContaining<CreateApiClientCommandValidator>();

            // Register SignalR
            builder.Services.RegisterSignalRServices();

            // Register MediatR
            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(ValidationPipelineBehaviour<,>).Assembly);
                config.AddOpenBehavior(typeof(ValidationPipelineBehaviour<,>));
                config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
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
            builder.Services.AddScoped<IApiClientSession, ApiClientSession>();
            builder.Services.AddScoped<IAddressValidationService, AddressValidationService>();
            builder.Services.AddScoped<IApiClientSession, ApiClientSession>();
            builder.Services.AddScoped<IJwtSession, JwtSession>();
            builder.Services.AddScoped<IGeoInfoService, GeoInfoService>();

            builder.Services.AddScoped<IDistanceService, DistanceService>();

            builder.Services.AddScoped<IPaymentMailTemplatesService, PaymentMailTemplatesService>();
            builder.Services.AddScoped<IShipmentMailTemplatesService, ShipmentMailTemplatesService>();

            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IStripeService, StripeService>();
            #endregion

            // Register Options pattern
            #region Register Options
            builder.Services.Configure<APISettings>(APISettings.GeoCodeAPI,
                    builder.Configuration.GetSection("API:GeoCodeAPI"));

            builder.Services.Configure<ShippingRatesSettings>(builder.Configuration.GetSection("ShippingRatesSettings"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailConfiguration"));
            builder.Services.Configure<PaymentGatewaySettings>(builder.Configuration.GetSection("PaymentGateways"));

            builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("Token"));
            #endregion
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

            app.MapHubs();
        }
    }

}
