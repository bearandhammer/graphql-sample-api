using GraphQL;
using GraphQL.Authorization;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using PizzaOrder.Business.Enums;
using PizzaOrder.Business.Helpers;
using PizzaOrder.Business.Interfaces;
using PizzaOrder.Business.Models;
using PizzaOrder.Data.Entities;
using PizzaOrder.GraphQLModels.InputTypes;
using PizzaOrder.GraphQLModels.Types;
using System.Collections.Generic;
using System.Linq;

namespace PizzaOrder.GraphQLModels.Queries
{
    public class PizzaOrderQuery : ObjectGraphType
    {
        public PizzaOrderQuery(IOrderDetailsService orderDetailsService, IPizzaDetailsService pizzaDetailsService)
        {
            Name = nameof(PizzaOrderQuery);
            this.AuthorizeWith(AuthPolicy.CustomerPolicy);

            FieldAsync<ListGraphType<OrderDetailsType>>(
                name: "newOrders",
                resolve: async context => await orderDetailsService.GetAllNewOrdersAsync())
            .AuthorizeWith(AuthPolicy.RestaurantPolicy);

            FieldAsync<PizzaDetailsType>(
                name: "pizzaDetails",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: async context => await pizzaDetailsService.GetPizzaDetailsAsync(
                    context.GetArgument<int>("id")));

            FieldAsync<OrderDetailsType>(
                name: "orderDetails",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: async context => await orderDetailsService.GetOrderDetailsAsync(
                    context.GetArgument<int>("id")))
            .AuthorizeWith(AuthPolicy.AdminPolicy);

            Connection<OrderDetailsType>()
                .Name("completedOrder")
                .Unidirectional()
                .PageSize(10)
                .Argument<CompletedOrderOrderByInputType>("orderBy", "Pass field & direction on which you want to sort data")
                .ResolveAsync(async context =>
                {
                    PageRequest pageRequest = new PageRequest
                    {
                        First = context.First,
                        Last = context.Last,
                        After = context.After,
                        Before = context.Before,
                        OrderBy = context.GetArgument<SortingDetails<CompletedOrdersSortingFields>>("orderBy")
                    };

                    PageResponse<OrderDetails> pageResponse = await orderDetailsService.GetCompletedOrdersAsync(pageRequest);

                    if (pageResponse.TotalCount == 0)
                    {
                        return new Connection<OrderDetails>()
                        {
                            PageInfo = new PageInfo()
                        };
                    }

                    (string startCursor, string endCursor) = CursorHelper.GetFirstAndLastCursor(
                        pageResponse.Nodes.Select(x => x.Id));

                    List<Edge<OrderDetails>> edge = pageResponse.Nodes.Select(x => new Edge<OrderDetails>
                    {
                        Cursor = CursorHelper.ToCursor(x.Id),
                        Node = x
                    })
                    .ToList();

                    Connection<OrderDetails> connection = new Connection<OrderDetails>()
                    {
                        Edges = edge,
                        TotalCount = pageResponse.TotalCount,
                        PageInfo = new PageInfo
                        {
                            HasNextPage = pageResponse.HasNextPage,
                            HasPreviousPage = pageResponse.HasPreviousPage,
                            StartCursor = startCursor,
                            EndCursor = endCursor
                        }
                    };

                    return connection;
                });

            Field<PizzaDetailsType>(
                name: "exceptionDemo",
                resolve: context =>
                {
                    Dictionary<string, string> data = new Dictionary<string, string>
                    {
                        { "Key", "Value" }
                    };

                    ExecutionError ex = new ExecutionError("A helpful error message", data);

                    // Fictitious!
                    ex.AddLocation(20, 50);
                    context.Errors.Add(ex);

                    return pizzaDetailsService.GetPizzaDetailsOrError();
                });
        }
    }
}
