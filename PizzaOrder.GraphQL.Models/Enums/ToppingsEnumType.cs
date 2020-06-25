using GraphQL.Types;
using PizzaOrder.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaOrder.GraphQL.Models.Enums
{
    public class ToppingsEnumType : EnumerationGraphType<Toppings>
    {
        public ToppingsEnumType()
        {
            Name = nameof(ToppingsEnumType);
        }
    }
}
