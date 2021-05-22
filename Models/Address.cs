using System.Collections.Generic;
using Newtonsoft.Json;
using pwr_msi.Models.Dto;

namespace pwr_msi.Models {
    public class Address {
        public int AddressId { get; set; }
        public string Addressee { get; set; }
        public string FirstLine { get; set; }
        public string SecondLine { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }

        public AddressDto AsDto() => new AddressDto() {
            AddressId = AddressId,
            Addressee = Addressee,
            FirstLine = FirstLine,
            SecondLine = SecondLine,
            PostCode = PostCode,
            City = City,
            Country = Country
        };
    }
}
