namespace pwr_msi.Models {
    public class OrderItem {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes;

        public Order Order { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}
