namespace pwr_msi.Models.Dto.Auth {
    public class UserProfileDto {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }

        public Address BillingAddress { get; set; }
    }
}
