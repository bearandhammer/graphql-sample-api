using GraphQL;
using GraphQL.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PizzaOrder.Business.Interfaces;
using PizzaOrder.Business.Services;
using PizzaOrder.Data;
using PizzaOrder.GraphQLModels.Enums;
using PizzaOrder.GraphQLModels.InputTypes;
using PizzaOrder.GraphQLModels.Mutation;
using PizzaOrder.GraphQLModels.Queries;
using PizzaOrder.GraphQLModels.Schema;
using PizzaOrder.GraphQLModels.Subscriptions;
using PizzaOrder.GraphQLModels.Types;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace PizzaOrder.API.Extensions
{
    public static class ConfigureServiceExtensions
    {
        public static void AddCustomIdentityAuth(this IServiceCollection services)
        {
            // Added Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<PizzaDBContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password configs
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // ApplicationUser settings
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
            });
        }

        public static void AddCustomJWT(this IServiceCollection services, IConfiguration configuration)
        {
            // Added JWT Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configuration.GetSection("JwtIssuerOptions:SecretKey").Value));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(configureOptions =>
            {
                configureOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection("JwtIssuerOptions:Issuer").Value,
                    ValidAudience = configuration.GetSection("JwtIssuerOptions:Audience").Value,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IPizzaDetailsService, PizzaDetailsService>();
            services.AddTransient<IOrderDetailsService, OrderDetailsService>();
            services.AddTransient<IEventService, EventService>();
        }

        public static void AddCustomGraphQLServices(this IServiceCollection services)
        {
            services.AddScoped<IServiceProvider>(c => new FuncServiceProvider(type => c.GetRequiredService(type)));
            services.AddGraphQL(options =>
            {
                options.EnableMetrics = true;
                options.ExposeExceptions = false;                   // false prints messages only, true will ToString()
                options.UnhandledExceptionDelegate = context =>
                {
                    Console.WriteLine("Error: " + context.OriginalException.Message);
                };
            })
            .AddWebSockets()
            .AddDataLoader()
            .AddGraphTypes(typeof(PizzaOrderSchema));
        }

        public static void AddCustomGraphQLTypes(this IServiceCollection services)
        {
            // Types
            services.AddSingleton<OrderDetailsType>();
            services.AddSingleton<PizzaDetailsType>();
            services.AddSingleton<EventDataType>();

            // Supported Enums
            services.AddSingleton<OrderStatusEnumType>();
            services.AddSingleton<ToppingsEnumType>();
            services.AddSingleton<CompletedOrdersSortingFieldsEnumType>();
            services.AddSingleton<SortingDirectionEnumType>();

            // Input Types
            services.AddSingleton<OrderDetailsInputType>();
            services.AddSingleton<PizzaDetailsInputType>();
            services.AddSingleton<CompletedOrderOrderByInputType>();

            // Queries/Mutations
            services.AddSingleton<PizzaOrderQuery>();
            services.AddSingleton<PizzaOrderMutation>();

            // Physical Schema
            services.AddSingleton<PizzaOrderSchema>();

            // Subscriptions
            services.AddSingleton<PizzaOrderSubscription>();
        }
    }
}
