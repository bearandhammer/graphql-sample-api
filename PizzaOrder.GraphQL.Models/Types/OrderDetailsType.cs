﻿using GraphQL.Types;
using PizzaOrder.Data.Entities;
using PizzaOrder.GraphQL.Models.Enums;

namespace PizzaOrder.GraphQL.Models.Types
{
    public class OrderDetailsType : ObjectGraphType<OrderDetails>
    {
        public OrderDetailsType()
        {
            Name = nameof(OrderDetailsType);
            Field(x => x.Id);
            Field(x => x.AddressLine1);
            Field(x => x.AddressLine2);
            Field(x => x.MobileNo);
            Field(x => x.Amount);
            Field(x => x.Date);
            Field<OrderStatusEnumType>(
                name: "orderStatus",
                resolve: context => context.Source.OrderStatus.ToString());
        }
    }
}
