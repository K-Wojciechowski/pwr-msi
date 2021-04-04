namespace pwr_msi.Models.Dto {
    public class RestaurantUserDto {
        public RestaurantBasicDto Restaurant { get; set; }
        public UserBasicDto User { get; set; }
        public bool CanManage { get; set; }
        public bool CanAcceptOrders { get; set; }
        public bool CanDeliverOrders { get; set; }
    }
}
