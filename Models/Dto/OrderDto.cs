using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using pwr_msi.Models.Enum;

namespace pwr_msi.Models.Dto {
    public class OrderDto {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryNotes { get; set; }
        public OrderStatus Status { get; set; }

        public ZonedDateTime Created { get; set; }
        public ZonedDateTime Updated { get; set; }

        public RestaurantBasicDto Restaurant { get; set; } = null!;
        public UserBasicDto Customer { get; set; } = null!;
        public Address Address { get; set; } = null!;

        public OrderTaskType LastTaskType => OrderTaskTypeSettings.taskTypeByStatus[Status];
        public ICollection<OrderItemDto> Items { get; set; }

        public async Task<Order> AsNewOrder(int userId, MsiDbContext dbContext) {
            // ReSharper disable once PossibleInvalidOperationException
            var menuItemIds = Items.Select(i => i.MenuItem.MenuItemId.Value).ToList();
            var menuItemOptionItemIds = Items.SelectMany(i =>
                i.Customizations.Select(c => c.MenuItemOptionItem.MenuItemOptionItemId)).ToList();

            var menuItemsWithPrices = from mi in dbContext.MenuItems
                where menuItemIds.Contains(mi.MenuItemId)
                select new {mi.MenuItemId, mi.Price};

            var menuItemOptionItemsWithPrices = from oi in dbContext.MenuItemOptionItems
                where menuItemOptionItemIds.Contains(oi.MenuItemOptionItemId)
                select new {oi.MenuItemOptionItemId, oi.Price};

            var menuItemPriceMap = await menuItemsWithPrices.ToDictionaryAsync(r => r.MenuItemId, r => r.Price);
            var menuItemOptionItemPriceMap =
                await menuItemOptionItemsWithPrices.ToDictionaryAsync(r => r.MenuItemOptionItemId, r => r.Price);

            var totalPrice = Items.Sum(item => {
                // ReSharper disable once PossibleInvalidOperationException
                var itemPrice = item.Amount * menuItemPriceMap[item.MenuItem.MenuItemId.Value];
                var customizationPrices = item.Customizations.Sum(cust =>
                    menuItemOptionItemPriceMap[cust.MenuItemOptionItem.MenuItemOptionItemId]);
                return itemPrice + customizationPrices;
            });
            var now = Utils.Now();
            return new Order {
                RestaurantId = Restaurant.RestaurantId,
                CustomerId = userId,
                AddressId = Address.AddressId,
                TotalPrice = totalPrice,
                DeliveryNotes = DeliveryNotes,
                Status = OrderStatus.CREATED,
                Created = now,
                // ReSharper disable once PossibleInvalidOperationException
                Items = Items.Select(i => i.AsNewOrderItem(menuItemPriceMap[i.MenuItem.MenuItemId.Value])).ToList(),
            };
        }
    }
}
