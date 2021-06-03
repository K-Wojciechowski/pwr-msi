#nullable enable
namespace pwr_msi.Models.Dto {
    public class RestaurantBasicDto {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }

        public RestaurantBasicDto(int restaurantId, string name, string? logo = null) {
            RestaurantId = restaurantId;
            Name = name;
            Logo = logo;
        }
    }
}
