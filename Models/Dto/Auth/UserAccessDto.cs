using System.Collections.Generic;

namespace pwr_msi.Models.Dto.Auth {
    public class UserAccessDto {
        public UserProfileDto Profile { get; set; }
        public bool Admin { get; set; }
        public IEnumerable<RestaurantBasicDto> Manage { get; set; }
        public IEnumerable<RestaurantBasicDto> Accept { get; set; }
        public IEnumerable<RestaurantBasicDto> Deliver { get; set; }
    }
}
