using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using PizzaOrder.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaOrder.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Loading...");
            Thread.Sleep(7500);

            Console.WriteLine("Client ready - press any key to run a test query...");
            Console.ReadKey();
            Console.WriteLine();

            GraphQLHttpClient graphQLClient = new GraphQLHttpClient("https://localhost:44311/graphql");

            string newOrdersQuery = @"
                query {
                    newOrders {
                        addressLine1
                        addressLine2
                        amount
                    }
                }";

            GraphQLResponse newOrderResponse = await graphQLClient.SendQueryAsync(newOrdersQuery);

            List<NewOrderDetails> newOrderDetails =
                newOrderResponse.GetDataFieldAs<List<NewOrderDetails>>("newOrders");

            if (newOrderDetails?.Any() ?? false)
            {
                newOrderDetails.ForEach(od =>
                {
                    Console.WriteLine($"AD1: { od.AddressLine1 }");
                    Console.WriteLine($"AD2: { od.AddressLine2 }");
                    Console.WriteLine($"Amount: { od.Amount }{ Environment.NewLine }----------{ Environment.NewLine }");
                });
            }

            string createOrderMutation = @"
                mutation ($order: OrderDetailsInputType!) { 
                    createOrder(orderDetails: $order) {
                        id
                        orderStatus
                        addressLine1
                        addressLine2
                        pizzaDetails {
                            id
                            toppings
                        }
                    }
                }
                ";

            GraphQLRequest createOrderRequest = new GraphQLRequest
            {
                Query = createOrderMutation,
                Variables = new
                {
                    order = new
                    {
                        addressLine1 = "1 Address Line",
                        addressLine2 = "2 Address Line",
                        mobileNo = "07854245685",
                        amount = 500,
                        pizzaDetails = new[]
                        {
                            new
                            {
                                name = "Super Cheesy Pizza",
                                price = 10,
                                size = 5,
                                toppings = "EXTRA_CHEESE"
                            }
                        }
                    }
                }
            };

            GraphQLResponse createOrderResponse = await graphQLClient.SendMutationAsync(createOrderRequest);

            CreateOrderDetails orderDetails = createOrderResponse.GetDataFieldAs<CreateOrderDetails>("createOrder");

            if (orderDetails != null)
            {
                Console.WriteLine($"Mutation Details{ Environment.NewLine }----------{ Environment.NewLine }");
                Console.WriteLine($"Id: { orderDetails.Id }");
                Console.WriteLine($"AD1: { orderDetails.AddressLine1 }");
                Console.WriteLine($"AD2: { orderDetails.AddressLine2 }");
                Console.WriteLine($"Order Status: { orderDetails.OrderStatus }");

                PizzaDetail pizzaDetail = orderDetails.PizzaDetails.FirstOrDefault();

                if (pizzaDetail != null)
                {
                    Console.WriteLine($"{ Environment.NewLine }Pizza Details{ Environment.NewLine }---{ Environment.NewLine }");

                    // Just the one...
                    Console.WriteLine($"Id: { pizzaDetail.Id }");
                    Console.WriteLine($"Toppings: { pizzaDetail.Toppings }");
                }

                Console.WriteLine();
            }

            Console.WriteLine("Processing complete");
        }
    }
}
