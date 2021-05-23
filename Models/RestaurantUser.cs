using pwr_msi.Models.Dto;

namespace pwr_msi.Models {
    public class RestaurantUser {
        public int RestaurantId { get; set; }
        public int UserId { get; set; }
        public bool CanManage { get; set; }
        public bool CanAcceptOrders { get; set; }
        public bool CanDeliverOrders { get; set; }

        public Restaurant Restaurant { get; set; }
        public User User { get; set; }

        public RestaurantUserDto AsDto() => new() {
            Restaurant = Restaurant.AsBasicDto(),
            User = User.AsBasicDto(),
            CanManage = CanManage,
            CanAcceptOrders = CanAcceptOrders,
            CanDeliverOrders = CanDeliverOrders,
        };
    }
}
