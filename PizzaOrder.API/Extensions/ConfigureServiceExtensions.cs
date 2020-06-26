﻿using GraphQL;
using GraphQL.Server;
using Microsoft.Extensions.DependencyInjection;
using PizzaOrder.Business.Interfaces;
using PizzaOrder.Business.Services;
using PizzaOrder.GraphQLModels.Enums;
using PizzaOrder.GraphQLModels.InputTypes;
using PizzaOrder.GraphQLModels.Mutation;
using PizzaOrder.GraphQLModels.Queries;
using PizzaOrder.GraphQLModels.Schema;
using PizzaOrder.GraphQLModels.Types;
using System;

namespace PizzaOrder.API.Extensions
{
    public static class ConfigureServiceExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IPizzaDetailsService, PizzaDetailsService>();
            services.AddTransient<IOrderDetailsService, OrderDetailsService>();
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

            // Supported Enums
            services.AddSingleton<OrderStatusEnumType>();
            services.AddSingleton<ToppingsEnumType>();

            // Input Types
            services.AddSingleton<OrderDetailsInputType>();
            services.AddSingleton<PizzaDetailsInputType>();

            // Queries/Mutations
            services.AddSingleton<PizzaOrderQuery>();
            services.AddSingleton<PizzaOrderMutation>();

            // Physical Schema
            services.AddSingleton<PizzaOrderSchema>();
        }
    }
}
