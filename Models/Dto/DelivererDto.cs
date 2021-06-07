namespace pwr_msi.Models.Dto {
    public class DelivererDto {
        public UserBasicDto User { get; set; }
        public int ActiveTasks { get; set; }

        public DelivererDto(UserBasicDto user, int activeTasks) {
            User = user;
            ActiveTasks = activeTasks;
        }


        public DelivererDto(User user, int activeTasks) : this(user.AsBasicDto(), activeTasks) {
        }
    }
}
