using PizzaOrder.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaOrder.Business.Interfaces
{
    public interface IPizzaDetailsService
    {
        Task<PizzaDetails> GetPizzaDetailsAsync(int pizzaDetailsId);

        IEnumerable<PizzaDetails> GetAllPizzaDetailsForOrder(int orderId);
    }
}
