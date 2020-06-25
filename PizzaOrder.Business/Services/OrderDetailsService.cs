using PizzaOrder.Business.Interfaces;
using PizzaOrder.Data;

namespace PizzaOrder.Business.Services
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly PizzaDBContext dbContext;

        public OrderDetailsService(PizzaDBContext context)
        {
            dbContext = context;
        }
    }
}
