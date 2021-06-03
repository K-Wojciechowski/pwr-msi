namespace pwr_msi.Models.Dto {
    public class DelivererDto {
        public RestaurantBasicDto Restaurant { get; set; }
        public UserBasicDto User { get; set; }
        public int ActiveTasks { get; set; }
    }
}
