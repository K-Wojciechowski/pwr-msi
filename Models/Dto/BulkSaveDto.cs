using System.Collections.Generic;
using NodaTime;

namespace pwr_msi.Models.Dto {
    public class BulkSaveDto<T> {
        public List<T> Added { get; set; }
        public List<T> Edited { get; set; }
        public List<T> Deleted { get; set; }
        public ZonedDateTime? ValidFrom { get; set; }
    }
}
