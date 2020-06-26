using GraphQL.Types;
using PizzaOrder.Business.Interfaces;
using PizzaOrder.Business.Models;
using PizzaOrder.Data.Entities;
using PizzaOrder.GraphQLModels.InputTypes;
using PizzaOrder.GraphQLModels.Types;
using System.Collections.Generic;
using System.Linq;

namespace PizzaOrder.GraphQLModels.Mutation
{
    public class PizzaOrderMutation : ObjectGraphType
    {
        public PizzaOrderMutation(IPizzaDetailsService pizzaDetailsService, IOrderDetailsService orderDetailsService)
        {
            Name = nameof(PizzaOrderMutation);

            FieldAsync<OrderDetailsType>(
                name: "createOrder",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<OrderDetailsInputType>>
                {
                    Name = "orderDetails"
                }),
                resolve: async context =>
                {
                    OrderDetailsModel order = context.GetArgument<OrderDetailsModel>("orderDetails");

                    OrderDetails newOrder = new OrderDetails(order.AddressLine1, order.AddressLine2,
                        order.MobileNo, order.Amount);

                    newOrder = await orderDetailsService.CreateAsync(newOrder);

                    IEnumerable<PizzaDetails> newPizzaDetails =
                        order.PizzaDetails.Select(pd => new PizzaDetails(pd.Name, pd.Toppings, pd.Price,
                            pd.Size, newOrder.Id));

                    newPizzaDetails = await pizzaDetailsService.CreateBulkAsync(newPizzaDetails, newOrder.Id);

                    newOrder.PizzaDetails = newPizzaDetails.ToList();

                    return newOrder;
                });
        }
    }
}
