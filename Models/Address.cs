using System.Collections.Generic;

namespace pwr_msi.Models {
    public class Address {
        public int AddressId { get; set; }
        public string Addressee { get; set; }
        public string FirstLine { get; set; }
        public string SecondLine { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
