using GraphQL.Resolvers;
using GraphQL.Types;
using PizzaOrder.Business.Interfaces;
using PizzaOrder.Business.Models;
using PizzaOrder.Data.Enums;
using PizzaOrder.GraphQLModels.Enums;
using PizzaOrder.GraphQLModels.Types;
using System.Reactive.Linq;

namespace PizzaOrder.GraphQLModels.Subscriptions
{
    public class PizzaOrderSubscription : ObjectGraphType
    {
        private readonly IEventService eventService;

        public PizzaOrderSubscription(IEventService evService)
        {
            eventService = evService;

            Name = nameof(PizzaOrderSubscription);

            AddField(new EventStreamFieldType
            {
                Name = "orderCreated",
                Type = typeof(EventDataType),
                Resolver = new FuncFieldResolver<EventDataModel>(context => context.Source as EventDataModel),
                Subscriber = new EventStreamResolver<EventDataModel>(context => 
                {
                    return eventService.OnCreateObservable;
                })
            });

            AddField(new EventStreamFieldType
            {
                Name = "statusUpdate",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<OrderStatusEnumType>>
                    {
                        Name = "status"
                    }),
                Type = typeof(EventDataType),
                Resolver = new FuncFieldResolver<EventDataModel>(context => context.Source as EventDataModel),
                Subscriber = new EventStreamResolver<EventDataModel>(context =>
                {
                    OrderStatus status = context.GetArgument<OrderStatus>("status");
                    var events = eventService.OnStatusUpdateObservable();

                    return events.Where(ev => ev.OrderStatus == status);
                })
            });
        }
    }
}
