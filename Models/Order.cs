namespace pwr_msi.Models {
    public class Order {
        public int OrderId { get; set; }
        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
        public int? DeliveryPersonId { get; set; }
        public int AddressId { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryNotes { get; set; }

        public Restaurant Restaurant { get; set; }
        public User Customer { get; set; }
        public User DeliveryPerson { get; set; }
        public Address Address { get; set; }
    }
}
