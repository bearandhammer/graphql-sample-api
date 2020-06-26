using PizzaOrder.Data.Entities;
using PizzaOrder.Data.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaOrder.Business.Interfaces
{
    public interface IOrderDetailsService
    {
        Task<OrderDetails> CreateAsync(OrderDetails orderDetails);

        Task<IEnumerable<OrderDetails>> GetAllNewOrdersAsync();

        Task<OrderDetails> GetOrderDetailsAsync(int orderId);

        Task<OrderDetails> UpdateStatusAsync(int orderId, OrderStatus orderStatus);
    }
}
