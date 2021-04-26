using System.Collections.Generic;
using Newtonsoft.Json;

namespace pwr_msi.Models.Dto {
    public class Page<T> {
        public IEnumerable<T> Items { get; set; }

        [JsonProperty(propertyName: "page")] public int PageNumber { get; set; }

        public int MaxPage { get; set; }

        public int ItemCount { get; set; }
    }
}
