using PizzaOrder.Business.Interfaces;
using PizzaOrder.Data;

namespace PizzaOrder.Business.Services
{
    public class PizzaDetailsService : IPizzaDetailsService
    {
        private readonly PizzaDBContext dbContext;

        public PizzaDetailsService(PizzaDBContext context)
        {
            dbContext = context;
        }
    }
}
