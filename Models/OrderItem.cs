using pwr_msi.Models.Dto;

namespace pwr_msi.Models {
    public class OrderItem {
        public string Notes;
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }

        public Order Order { get; set; }
        public MenuItem MenuItem { get; set; }

        public OrderItemDto AsDto() => new OrderItemDto() {
            Notes = Notes,
            Amount = Amount,
            TotalPrice = TotalPrice,
            MenuItem = MenuItem
        };
    }
}
