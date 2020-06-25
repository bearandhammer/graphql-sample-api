using Microsoft.Extensions.DependencyInjection;
using PizzaOrder.GraphQLModels.Queries;
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
        }
    }
}
