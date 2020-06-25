using GraphQL.Types;
using PizzaOrder.Business.Interfaces;
using PizzaOrder.GraphQLModels.Types;

namespace PizzaOrder.GraphQLModels.Queries
{
    public class PizzaOrderQuery : ObjectGraphType
    {
        public PizzaOrderQuery(IOrderDetailsService orderDetailsService)
        {
            Name = nameof(PizzaOrderQuery);

            FieldAsync<ListGraphType<OrderDetailsType>>(
                name: "newOrders",
                resolve: async context => await orderDetailsService.GetAllNewOrdersAsync());
        }
    }
}
