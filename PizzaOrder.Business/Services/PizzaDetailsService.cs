using PizzaOrder.Business.Interfaces;
using PizzaOrder.Data;
using PizzaOrder.Data.Entities;
using System.Threading.Tasks;

namespace PizzaOrder.Business.Services
{
    public class PizzaDetailsService : IPizzaDetailsService
    {
        private readonly PizzaDBContext dbContext;

        public PizzaDetailsService(PizzaDBContext context)
        {
            dbContext = context;
        }

        public async Task<PizzaDetails> GetPizzaDetailsAsync(int pizzaDetailsId)
        {
            return await dbContext.PizzaDetails
                .FindAsync(pizzaDetailsId);
        }
    }
}
