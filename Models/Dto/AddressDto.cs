namespace pwr_msi.Models.Dto {
    public class AddressDto {
        public int AddressId { get; set; }
        public string Addressee { get; set; }
        public string FirstLine { get; set; }
        public string SecondLine { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public Address AsNewAddress() => new Address() {
                Addressee = Addressee,
                City = City,
                Country = Country,
                FirstLine = FirstLine,
                SecondLine = SecondLine,
                PostCode = PostCode
        };
    }
}
