namespace pwr_msi.Models {
    public class Order {
        public int OrderId { get; set; }
        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
        public int? DeliveryPersonId { get; set; }
        public int AddressId { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryNotes { get; set; }
        public OrderStatus Status { get; set; }

        public virtual Restaurant Restaurant { get; set; }
        public virtual User Customer { get; set; }
        public virtual User DeliveryPerson { get; set; }
        public virtual Address Address { get; set; }
    }
}
