using Microsoft.EntityFrameworkCore;
using PizzaOrder.Business.Interfaces;
using PizzaOrder.Business.Models;
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
        private readonly IEventService eventService;

        public OrderDetailsService(PizzaDBContext context, IEventService evService)
        {
            dbContext = context;
            eventService = evService;
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

            // Trigger events for any subscriptions...
            eventService.AddOrderEvent(new EventDataModel(orderDetails.Id));

            return orderDetails;
        }

        public async Task<OrderDetails> UpdateStatusAsync(int orderId, OrderStatus orderStatus)
        {
            OrderDetails discoveredOrder = await dbContext.OrderDetails.FindAsync(orderId);

            if (discoveredOrder != null)
            {
                discoveredOrder.OrderStatus = orderStatus;
                await dbContext.SaveChangesAsync();

                eventService.StatusUpdateEvent(new EventDataModel(orderId, orderStatus));
            }

            return discoveredOrder;
        }
    }
}
