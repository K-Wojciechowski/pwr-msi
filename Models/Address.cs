using System.Collections.Generic;
using Newtonsoft.Json;

namespace pwr_msi.Models {
    public class Address {
        public int AddressId { get; set; }
        public string Addressee { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        [JsonIgnore]
        public ICollection<User> Users { get; set; }

        public void Update(Address address) {
            this.Addressee = address.Addressee;
            this.FirstLine = address.FirstLine;
            this.SecondLine = address.SecondLine;
            this.PostCode = address.PostCode;
            this.City = address.City;
            this.Country = address.Country;
        }
    }
}
