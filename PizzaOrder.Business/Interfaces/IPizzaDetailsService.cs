using PizzaOrder.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaOrder.Business.Interfaces
{
    public interface IPizzaDetailsService
    {
        Task<PizzaDetails> GetPizzaDetailsAsync(int pizzaDetailsId);

        IEnumerable<PizzaDetails> GetAllPizzaDetailsForOrder(int orderId);

        Task<IEnumerable<PizzaDetails>> CreateBulkAsync(IEnumerable<PizzaDetails> pizzaDetails, int orderId);

        Task<int> DeletePizzaDetailsAsync(int pizzaDetailsId);
    }
}
