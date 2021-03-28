using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace pwr_msi.Models {
    public class AppConfig {
        private static readonly AppConfig DefaultSource = new() {
            AuthTokenLifetime = TimeSpan.FromMinutes(value: 5),
            RefreshTokenLifetime = TimeSpan.FromDays(value: 7),
            ServerAddress = "http://localhost:5000/",
            DbConnectionString = "Host=localhost;Database=msi;Username=msi;Password=msi",
            EmailFromAddress = "noreply@msi.local",
            EmailFromName = "MSI",
            SmtpHost = "localhost",
            SmtpPort = 1025,
            SmtpAuthenticate = false,
        };

        public byte[] JwtKey { get; set; }
        public TimeSpan AuthTokenLifetime { get; set; }
        public TimeSpan RefreshTokenLifetime { get; set; }
        public string DbConnectionString { get; set; }
        public string ServerAddress { get; set; }
        public TokenValidationParameters JwtValidationParameters { get; set; }
        public string EmailFromName { get; set; }
        public string EmailFromAddress { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpAuthenticate { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }


        private static byte[] GetJwtKey(IConfiguration configuration) {
            var jwtKeyString = configuration.GetString(key: "JWT_KEY", defaultValue: null);
            if (jwtKeyString != null) return Encoding.UTF8.GetBytes(jwtKeyString);
            var jwtKey = new byte[32];
            var rngProvider = new RNGCryptoServiceProvider();
            rngProvider.GetNonZeroBytes(jwtKey);
            rngProvider.Dispose();
            return jwtKey;
        }

        public static AppConfig FromConfiguration(IConfiguration configuration) {
            var jwtKey = GetJwtKey(configuration);
            var serverAddress = configuration.GetString(key: "SERVER_ADDRESS", DefaultSource.ServerAddress);

            return new AppConfig {
                AuthTokenLifetime =
                    configuration.GetTimeSpan(key: "AUTH_TOKEN_LIFETIME", DefaultSource.AuthTokenLifetime),
                RefreshTokenLifetime =
                    configuration.GetTimeSpan(key: "AUTH_REFRESH_TOKEN_LIFETIME", DefaultSource.RefreshTokenLifetime),
                ServerAddress = serverAddress,
                DbConnectionString =
                    configuration.GetString(key: "DB_CONNECTION_STRING", DefaultSource.DbConnectionString),
                EmailFromAddress = configuration.GetString(key: "EMAIL_FROM_ADDRESS", DefaultSource.EmailFromAddress),
                EmailFromName = configuration.GetString(key: "EMAIL_FROM_NAME", DefaultSource.EmailFromName),
                SmtpHost = configuration.GetString(key: "SMTP_HOST", DefaultSource.SmtpHost),
                SmtpPort = configuration.GetInt(key: "SMTP_PORT", DefaultSource.SmtpPort),
                SmtpAuthenticate = configuration.GetBoolean(key: "SMTP_AUTHENTIATE", DefaultSource.SmtpAuthenticate),
                SmtpUsername = configuration.GetString(key: "SMTP_USERNAME", DefaultSource.SmtpUsername),
                SmtpPassword = configuration.GetString(key: "SMTP_PASSWORD", DefaultSource.SmtpPassword),
                JwtKey = jwtKey,
                JwtValidationParameters = new TokenValidationParameters {
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = serverAddress,
                    ValidAudience = serverAddress,
                    ClockSkew = TimeSpan.FromSeconds(value: 15),
                },
            };
        }
    }
}
