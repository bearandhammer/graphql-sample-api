using GraphQL.Types;

namespace PizzaOrder.GraphQLModels.Enums
{
    public class OrderStatusEnumType : EnumerationGraphType
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
