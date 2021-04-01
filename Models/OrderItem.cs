namespace pwr_msi.Models {
    public class OrderItem {
        public string Notes;
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }

        public virtual Order Order { get; set; }
        public virtual MenuItem MenuItem { get; set; }
    }
}
