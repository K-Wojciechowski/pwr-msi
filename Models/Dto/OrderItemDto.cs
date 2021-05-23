namespace pwr_msi.Models.Dto {
    public class OrderItemDto {
        public string Notes;
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public MenuItem MenuItem { get; set; }
        
        public OrderItem AsNewOrderItem() => new() {
            Notes = Notes,
            Amount = Amount,
            TotalPrice = TotalPrice,
            MenuItem = MenuItem
        };
    
    }
}
