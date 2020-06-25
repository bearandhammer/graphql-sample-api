using GraphQL.Types;
using PizzaOrder.Data.Enums;

namespace PizzaOrder.GraphQL.Models.Enums
{
    public class OrderStatusEnumType : EnumerationGraphType<OrderStatus>
    {
        public OrderStatusEnumType()
        {
            Name = "orderStatus";

            // TODO - grab this from the enum itself
            AddValue("Created", "Order created.", 1);
            AddValue("InKitchen", "Order is being prepared.", 2);
            AddValue("OnTheWay", "Order is on the way.", 3);
            AddValue("Delivered", "Order was delivered.", 4);
            AddValue("Cancelled", "Order was cancelled.", 5);
        }
    }
}
