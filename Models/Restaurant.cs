#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        public Address Address { get; set; } = null!;

        public ICollection<Cuisine> Cuisines { get; set; } = null!;
        public ICollection<MenuCategory> MenuCategories { get; set; } = null!;
        public ICollection<MenuItem> MenuItems { get; set; } = null!;
        public ICollection<User> Users { get; set; } = null!;
        public List<RestaurantUser> RestaurantUsers { get; set; } = null!;

        public RestaurantBasicDto AsBasicDto() => new (RestaurantId, Name, Logo);
        public RestaurantBasicAddressDto AsBasicAddressDto() => new (RestaurantId, Name, Address, Logo);

        public RestaurantFullDto AsAdminDto() => new () {
            RestaurantId = RestaurantId,
            Name = Name,
            Website = Website,
            Description = Description,
            Logo = Logo,
            Address = Address,
            IsActive = IsActive,
            Cuisines = Cuisines.ToList(),
        };
        public RestaurantDetailDto AsDetailDto() => new () {
            RestaurantId = RestaurantId,
            Logo = Logo,
            Cuisines = Cuisines,
            Name = Name,
            Website = Website,
            Description = Description,
            Address = Address,
        };

        public async Task UpdateWithAdminDto(RestaurantFullDto raDto, DbSet<Cuisine> cuisineSource) {
            Name = raDto.Name;
            Website = raDto.Website;
            Description = raDto.Description;
            Address.Update(raDto.Address);
            Logo = raDto.Logo;
            IsActive = raDto.IsActive;
            var cuisineIds = raDto.Cuisines.Select(c => c.CuisineId);
            Cuisines = await cuisineSource.Where(c => cuisineIds.Contains(c.CuisineId)).ToListAsync();
        }
    }
}
