using Microsoft.EntityFrameworkCore;
using PizzaOrder.Business.Interfaces;
using PizzaOrder.Data;
using PizzaOrder.Data.Entities;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<PizzaDetails> GetAllPizzaDetailsForOrder(int orderId)
        {
            return dbContext.PizzaDetails
                .Where(x => x.OrderDetailsId == orderId)
                .ToList();
        }

        public async Task<IEnumerable<PizzaDetails>> CreateBulkAsync(
            IEnumerable<PizzaDetails> pizzaDetails,
            int orderId)
        {
            await dbContext.PizzaDetails.AddRangeAsync(pizzaDetails);
            await dbContext.SaveChangesAsync();

            return dbContext.PizzaDetails.Where(x => x.OrderDetailsId == orderId);
        }
    }
}
