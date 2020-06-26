using Microsoft.Extensions.DependencyInjection;
using PizzaOrder.GraphQLModels.Mutation;
using PizzaOrder.GraphQLModels.Queries;
using PizzaOrder.GraphQLModels.Subscriptions;
using System;

namespace PizzaOrder.GraphQLModels.Schema
{
    public class PizzaOrderSchema : GraphQL.Types.Schema
    {
        public PizzaOrderSchema(IServiceProvider services)
            : base(services)
        {
            Services = services;
            Query = services.GetService<PizzaOrderQuery>();
            Mutation = services.GetService<PizzaOrderMutation>();
            Subscription = services.GetService<PizzaOrderSubscription>();
        }
    }
}
