namespace pwr_msi.Models.Dto.Admin {
    public class RestaurantAdminDto {
        public int? RestaurantId { get; init; }
        public string Name { get; init; }
        public string Website { get; init; }
        public string Description { get; init; }
        public Address Address { get; init; }

        public Restaurant AsNewRestaurant() => new() {
            Name = Name, Website = Website, Description = Description, Address = Address,
        };
    }
}
