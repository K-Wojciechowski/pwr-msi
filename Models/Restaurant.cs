#nullable enable
using System.Collections.Generic;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.Admin;

namespace pwr_msi.Models {
    public class Restaurant {
        public int RestaurantId { get; set; }
        public string Name { get; set; } = null!;
        public string? Website { get; set; }
        public string Description { get; set; } = null!;
        public string? Logo { get; set; }
        public bool IsActive { get; set; }

        public int AddressId { get; set; }
        public virtual Address Address { get; set; } = null!;

        public virtual ICollection<Cuisine> Cuisines { get; set; } = null!;
        public virtual ICollection<MenuCategory> MenuCategories { get; set; } = null!;
        public virtual ICollection<User> Users { get; set; } = null!;
        public virtual List<RestaurantUser> RestaurantUsers { get; set; } = null!;

        public RestaurantBasicDto AsBasicDto() => new() {RestaruantId = RestaurantId, Name = Name};

        public RestaurantAdminDto AsAdminDto() => new () {
            RestaurantId = RestaurantId,
            Name = Name,
            Website = Website,
            Description = Description,
            Logo = Logo,
            Address = Address,
            IsActive = IsActive,
        };

        public void UpdateWithAdminDto(RestaurantAdminDto raDto) {
            Name = raDto.Name;
            Website = raDto.Website;
            Description = raDto.Description;
            Address = raDto.Address;
            Logo = raDto.Logo;
            IsActive = raDto.IsActive;
        }
    }
}
