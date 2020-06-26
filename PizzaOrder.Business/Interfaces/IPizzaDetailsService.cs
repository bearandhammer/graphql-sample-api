using PizzaOrder.Data.Entities;
using System.Threading.Tasks;

namespace PizzaOrder.Business.Interfaces
{
    public interface IPizzaDetailsService
    {
        Task<PizzaDetails> GetPizzaDetailsAsync(int pizzaDetailsId);
    }
}
