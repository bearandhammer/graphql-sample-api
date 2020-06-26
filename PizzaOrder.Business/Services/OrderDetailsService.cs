using Microsoft.EntityFrameworkCore;
using PizzaOrder.Business.Interfaces;
using PizzaOrder.Data;
using PizzaOrder.Data.Entities;
using PizzaOrder.Data.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaOrder.Business.Services
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly PizzaDBContext dbContext;

        public OrderDetailsService(PizzaDBContext context)
        {
            dbContext = context;
        }

        public async Task<IEnumerable<OrderDetails>> GetAllNewOrdersAsync()
        {
            return await dbContext.OrderDetails
                .Where(x => x.OrderStatus == OrderStatus.Created)
                .ToListAsync();
        }

        public async Task<OrderDetails> GetOrderDetailsAsync(int orderId)
        {
            return await dbContext.OrderDetails.FindAsync(orderId);
        }

        public async Task<OrderDetails> CreateAsync(OrderDetails orderDetails)
        {
            dbContext.OrderDetails.Add(orderDetails);
            await dbContext.SaveChangesAsync();

            return orderDetails;
        }
    }
}
