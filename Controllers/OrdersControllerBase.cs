using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    public abstract class OrdersControllerBase: MsiControllerBase {
        protected MsiDbContext _dbContext;
        protected OrderDetailsService _orderDetailsService;
        protected abstract bool IncludeDeliveryPerson { get; }

        protected async Task<List<OrderBasicDto>> GetBasicOrders(IQueryable<Order> orders) {
            var ids = await orders.OrderByDescending(o => o.Updated).Select(o => o.OrderId).ToListAsync();
            return await _orderDetailsService.GetBasicOrdersByIds(ids, IncludeDeliveryPerson);
        }

        protected async Task<List<OrderBasicDto>> GetBasicOrders(List<Order> orders) {
            var ids = orders.Select(o => o.OrderId);
            return await _orderDetailsService.GetBasicOrdersByIds(ids, IncludeDeliveryPerson);
        }

        protected async Task<Page<OrderBasicDto>> PaginateBasicOrders(IQueryable<Order> query, int page) {
            var queryOrdered = query.OrderByDescending(o => o.Updated);
            return await Utils.PaginateAsyncNullable(queryOrdered, page,
                async o =>
                    await _orderDetailsService.GetBasicOrderById(o.OrderId, IncludeDeliveryPerson));
        }
    }
}
