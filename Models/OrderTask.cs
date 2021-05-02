#nullable enable
using NodaTime;
using pwr_msi.Models.Enum;

namespace pwr_msi.Models {
    public class OrderTask {
        public int OrderTaskId { get; set; }
        public OrderTaskType Task { get; set; }
        public ZonedDateTime? DateCompleted { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        public int? CompletedById { get; set; }
        public virtual User? CompletedBy { get; set; }

        public int? AssigneeUserId { get; set; }
        public virtual User? AssigneeUser { get; set; }

        public int? AssigneeRestaurantId { get; set; }
        public virtual Restaurant? AssigneeRestaurant { get; set; }

        public AssigneeType? AssigneeType { get; set; }
    }
}
