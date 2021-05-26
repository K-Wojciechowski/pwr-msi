using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using pwr_msi.Models.Dto;
using pwr_msi.Serialization;

#nullable enable
namespace pwr_msi.Services {
    public class OrderDetailsService {
        private readonly MsiDbContext _dbContext;
        private readonly IDistributedCache _cache;


        public OrderDetailsService(MsiDbContext dbContext, IDistributedCache cache) {
            _dbContext = dbContext;
            _cache = cache;
        }

        private async Task<OrderDto?> GetOrderFromCache(int orderId) {
            var orderString = await _cache.GetStringAsync("order:" + orderId);
            if (orderString == null) return null;
            return JsonConvert.DeserializeObject<OrderDto>(orderString, MsiSerializerSettings.jsonSerializerSettings);
        }

        private async Task<OrderDto?> GetOrderFromDbAndSaveInCache(int orderId) {
            var queryable = _dbContext.Orders
                .Include(o => o.Address)
                .Include(o => o.Customer)
                .Include(o => o.Restaurant)
                .Include(o => o.DeliveryPerson)
                .Include("Items")
                .Include("Items.MenuItem")
                .Include("Items.Customizations")
                .Include("Items.Customizations.MenuItemOptionItem");
            var order = await queryable.Where(o => o.OrderId == orderId).FirstOrDefaultAsync();
            if (order == null) return null;

            var orderDto = order.AsDto();
            var orderString = JsonConvert.SerializeObject(orderDto, MsiSerializerSettings.jsonSerializerSettings);
            await _cache.SetStringAsync("order:" + orderId, orderString);

            return orderDto;
        }

        public async Task<OrderDto?> GetOrderById(int orderId) {
            var cachedOrder = await GetOrderFromCache(orderId);
            if (cachedOrder == null) {
                return await GetOrderFromDbAndSaveInCache(orderId);
            }

            var dbUpdatedQuery = from o in _dbContext.Orders
                where o.OrderId == orderId
                select o.Updated;
            var hasAnyData = await dbUpdatedQuery.AnyAsync();
            if (!hasAnyData) return null;
            var dbUpdated = await dbUpdatedQuery.FirstOrDefaultAsync();

            if (cachedOrder.Updated == dbUpdated) {
                return cachedOrder;
            }
            return await GetOrderFromDbAndSaveInCache(orderId);

        }
    }
}
