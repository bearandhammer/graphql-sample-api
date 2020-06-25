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
    }
}
