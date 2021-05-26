#nullable enable
using System.Collections.Generic;
using System.Linq;
using pwr_msi.Models.Dto;

namespace pwr_msi.Models {
    public class OrderItem {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; } = null!;

        public Order Order { get; set; } = null!;
        public MenuItem MenuItem { get; set; } = null!;
        public ICollection<OrderItemCustomization> Customizations { get; set; } = null!;

        public OrderItemDto AsDto() => new OrderItemDto() {
            Notes = Notes,
            Amount = Amount,
            TotalPrice = TotalPrice,
            MenuItem = MenuItem.AsDto(),
            Customizations = Customizations.Select(c => c.AsDto()).ToList(),
        };
    }
}
