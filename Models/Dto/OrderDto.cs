#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using pwr_msi.Models.Enum;

namespace pwr_msi.Models.Dto {
    public class OrderDto {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryNotes { get; set; } = null!;
        public OrderStatus Status { get; set; }

        public ZonedDateTime Created { get; set; }
        public ZonedDateTime Updated { get; set; }
        public ZonedDateTime? Delivered { get; set; }

        public RestaurantBasicDto Restaurant { get; set; } = null!;
        public UserBasicDto? Customer { get; set; } = null!;
        public UserBasicDto? DeliveryPerson { get; set; }
        public Address Address { get; set; } = null!;

        public ICollection<OrderItemDto> Items { get; set; } = null!;

        public async Task<Order> AsNewOrder(int userId, MsiDbContext dbContext) {
            var menuItemIds = Items.Select(i => i.MenuItem.MenuItemId!.Value).ToList();
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
                var itemPrice = item.Amount * menuItemPriceMap[item.MenuItem.MenuItemId!.Value];
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
                Items = Items.Select(i => i.AsNewOrderItem(menuItemPriceMap[i.MenuItem.MenuItemId!.Value])).ToList(),
            };
        }

        public OrderBasicDto AsBasicDto() {
            Debug.Assert(Customer != null, nameof(Customer) + " != null");
            return new OrderBasicDto {
               OrderId = OrderId,
               Restaurant = Restaurant,
               Customer = Customer,
               Address = Address,
               Status = Status,
               TotalPrice = TotalPrice,
               DeliveryNotes = DeliveryNotes,
               ItemNames = Items.ToList().Select(i => i.MenuItem.Name).ToList(),

               Created = Created,
               Updated = Updated,
               Delivered = Delivered,
            };
        }
    }
}
