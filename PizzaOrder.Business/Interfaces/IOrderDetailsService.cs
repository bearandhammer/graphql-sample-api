using PizzaOrder.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaOrder.Business.Interfaces
{
    public interface IOrderDetailsService
    {
        Task<IEnumerable<OrderDetails>> GetAllNewOrdersAsync();

        Task<OrderDetails> GetOrderDetailsAsync(int orderId);
    }
}
