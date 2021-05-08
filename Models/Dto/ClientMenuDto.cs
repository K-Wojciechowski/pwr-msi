using System.Collections.Generic;


namespace pwr_msi.Models.Dto {
    public class ClientMenuDto {
        public string Name { get; set; }
        public virtual ICollection<MenuItem> Items { get; set; }
    }
}
