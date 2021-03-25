namespace pwr_msi.Models.Dto {
    public class AuthInput {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RefreshInput {
        public string RefreshToken { get; set; }
    }

    public class AuthResult {
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshIn { get; set; }
    }

    public class RefreshResult {
        public string AuthToken { get; set; }
        public int RefreshIn { get; set; }
    }
}
