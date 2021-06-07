#nullable enable
namespace pwr_msi.Models.Dto {
    public class RestaurantBasicAddressDto {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }
        public Address? Address { get; set; }

        public RestaurantBasicAddressDto(int restaurantId, string name, Address? address = null, string? logo = null) {
            RestaurantId = restaurantId;
            Name = name;
            Logo = logo;
            Address = address;
        }

        public RestaurantBasicDto AsBasicDto() => new(RestaurantId, Name, Logo);
    }
}
