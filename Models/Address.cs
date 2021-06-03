using System.Collections.Generic;
using Newtonsoft.Json;

namespace pwr_msi.Models {
    public class Address {
        public int AddressId { get; set; }
        public string Addressee { get; set; }
        public string FirstLine { get; set; }
        public string SecondLine { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        [JsonIgnore]
        public ICollection<User> Users { get; set; }

        public void Update(Address address) {
            Addressee = address.Addressee;
            FirstLine = address.FirstLine;
            SecondLine = address.SecondLine;
            PostCode = address.PostCode;
            City = address.City;
            Country = address.Country;
            Latitude = address.Latitude;
            Longitude = address.Longitude;
        }
    }
}
