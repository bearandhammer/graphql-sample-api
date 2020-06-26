using Microsoft.EntityFrameworkCore;
using PizzaOrder.Business.Helpers;
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

        public async Task<PageResponse<OrderDetails>> GetCompletedOrdersAsync(PageRequest pageRequest)
        {
            IQueryable<OrderDetails> filterQuery = dbContext.OrderDetails
                .Where(x => x.OrderStatus == OrderStatus.Delivered);

            IQueryable<OrderDetails> dataQuery = filterQuery;
            if (pageRequest.First.HasValue)
            {
                if (!string.IsNullOrEmpty(pageRequest.After))
                {
                    int lastId = CursorHelper.FromCursor(pageRequest.After);
                    dataQuery = dataQuery.Where(x => x.Id > lastId);
                }

                dataQuery = dataQuery.Take(pageRequest.First.Value);
            }

            if (dataQuery.Count() == 0)
            {
                return new PageResponse<OrderDetails>();
            }

            if (pageRequest.OrderBy?.Field == Enums.CompletedOrdersSortingFields.Address)
            {
                dataQuery = (pageRequest.OrderBy.Direction == Enums.SortingDirection.Desc)
                    ? dataQuery.OrderByDescending(x => x.AddressLine1)
                    : dataQuery.OrderBy(x => x.AddressLine1);
            }
            else if (pageRequest.OrderBy?.Field == Enums.CompletedOrdersSortingFields.Amount)
            {
                dataQuery = (pageRequest.OrderBy.Direction == Enums.SortingDirection.Desc)
                    ? dataQuery.OrderByDescending(x => x.Amount)
                    : dataQuery.OrderBy(x => x.Amount);
            }
            else
            {
                dataQuery = (pageRequest.OrderBy.Direction == Enums.SortingDirection.Desc)
                    ? dataQuery.OrderByDescending(x => x.Id)
                    : dataQuery.OrderBy(x => x.Id);
            }

            List<OrderDetails> nodes = await dataQuery.ToListAsync();

            int maxId = nodes.Max(x => x.Id);
            int minId = nodes.Min(x => x.Id);
            bool hasNextPage = await filterQuery.AnyAsync(x => x.Id > maxId);
            bool hasPrevPage = await filterQuery.AnyAsync(x => x.Id < minId);
            int totalCount = await filterQuery.CountAsync();

            return new PageResponse<OrderDetails>
            {
                Nodes = nodes,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPrevPage,
                TotalCount = totalCount
            };
        }
    }
}
