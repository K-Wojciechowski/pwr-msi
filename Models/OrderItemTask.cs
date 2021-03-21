using System;

namespace pwr_msi.Models {
    public class OrderItemTask {
        public int OrderItemTaskId { get; set; }
        public OrderItemTaskType Task { get; set; }
        public DateTime DateCompleted { get; set; }
        public int CompletedById { get; set; }
        public User CompletedBy { get; set; }
    }
}
