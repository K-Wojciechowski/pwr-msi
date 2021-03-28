using NodaTime;

namespace pwr_msi.Models {
    public class OrderTask {
        public int OrderTaskId { get; set; }
        public OrderTaskType Task { get; set; }
        public ZonedDateTime DateCompleted { get; set; }
        public int CompletedById { get; set; }
        public User CompletedBy { get; set; }

        public int AssigneeUserId { get; set; }
        public User AssigneeUser { get; set; }

        public int AssigneeRestaurantId { get; set; }
        public Restaurant AssigneeRestaurant { get; set; }

        public AssigneeType AssigneeType { get; set; }
    }
}
