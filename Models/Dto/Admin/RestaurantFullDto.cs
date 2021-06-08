using System.Collections.Generic;

namespace pwr_msi.Models.Dto.Admin {
    public class RestaurantFullDto {
        public int? RestaurantId { get; init; }
        public string Name { get; init; }
        public string Website { get; init; }
        public string Description { get; init; }
        public string Logo { get; init; }
        public Address Address { get; init; }
        public bool IsActive { get; init; }
        public List<Cuisine> Cuisines { get; init; }

        public Restaurant AsNewRestaurant() => new() {
            Name = Name, Website = Website, Description = Description, Logo = Logo, Address = Address, IsActive = IsActive, Cuisines = Cuisines,
        };
    }
}
