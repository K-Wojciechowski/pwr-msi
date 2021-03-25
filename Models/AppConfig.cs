using System;
using Microsoft.IdentityModel.Tokens;

namespace pwr_msi.Models {
    public class AppConfig {
        public byte[] JwtKey { get; set; }
        public TimeSpan AuthTokenLifetime { get; set; }
        public TimeSpan RefreshTokenLifetime { get; set; }
        public string ServerAddress { get; set; }
        public TokenValidationParameters JwtValidationParameters { get; set; }
    }
}
