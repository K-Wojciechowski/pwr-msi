using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Amazon;

namespace pwr_msi.Models {
    public class AppConfig {
        private static readonly AppConfig DefaultSource = new() {
            AuthTokenLifetime = TimeSpan.FromMinutes(value: 5),
            RefreshTokenLifetime = TimeSpan.FromDays(value: 7),
            ServerAddress = "http://localhost:5000/",
            DbConnectionString = "Host=localhost;Database=msi;Username=msi;Password=msi",
            PayUrl = "http://localhost:5007/",
            EmailFromAddress = "noreply@msi.local",
            EmailFromName = "MSI",
            RedisConnectionString = "localhost",
            RedisInstanceName = "msi",
            SmtpHost = "localhost",
            SmtpPort = 1025,
            SmtpAuthenticate = false,
            S3BucketName = "msiuploads",
            S3Url = "http://localhost:9000/",
            S3Region = RegionEndpoint.USEast1,
            S3AccessKey = "msi_s3_root_user",
            S3SecretKey = "msi_s3_root_password",
        };

        public byte[] JwtKey { get; set; }
        public TimeSpan AuthTokenLifetime { get; set; }
        public TimeSpan RefreshTokenLifetime { get; set; }
        public string DbConnectionString { get; set; }
        public string ServerAddress { get; set; }
        public TokenValidationParameters JwtValidationParameters { get; set; }
        public string PayUrl { get; set; }
        public string EmailFromName { get; set; }
        public string EmailFromAddress { get; set; }
        public string RedisConnectionString { get; set; }
        public string RedisInstanceName { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpAuthenticate { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string S3BucketName { get; set; }
        public string S3Url { get; set; }
        public RegionEndpoint S3Region { get; set; }
        public string S3AccessKey { get; set; }
        public string S3SecretKey { get; set; }

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

            var s3RegionName = configuration.GetString(key: "S3_REGION", defaultValue: null);
            var s3Region = s3RegionName == null ? DefaultSource.S3Region : RegionEndpoint.GetBySystemName(s3RegionName);

            return new AppConfig {
                AuthTokenLifetime =
                    configuration.GetTimeSpan(key: "AUTH_TOKEN_LIFETIME", DefaultSource.AuthTokenLifetime),
                RefreshTokenLifetime =
                    configuration.GetTimeSpan(key: "AUTH_REFRESH_TOKEN_LIFETIME", DefaultSource.RefreshTokenLifetime),
                ServerAddress = serverAddress,
                DbConnectionString =
                    configuration.GetString(key: "DB_CONNECTION_STRING", DefaultSource.DbConnectionString),
                PayUrl = configuration.GetString(key: "PAY_URL", DefaultSource.PayUrl),
                EmailFromAddress = configuration.GetString(key: "EMAIL_FROM_ADDRESS", DefaultSource.EmailFromAddress),
                EmailFromName = configuration.GetString(key: "EMAIL_FROM_NAME", DefaultSource.EmailFromName),
                RedisConnectionString = configuration.GetString("REDIS_CONNECTION_STRING", DefaultSource.RedisConnectionString),
                RedisInstanceName = configuration.GetString("REDIS_INSTANCE_NAME", DefaultSource.RedisInstanceName),
                SmtpHost = configuration.GetString(key: "SMTP_HOST", DefaultSource.SmtpHost),
                SmtpPort = configuration.GetInt(key: "SMTP_PORT", DefaultSource.SmtpPort),
                SmtpAuthenticate = configuration.GetBoolean(key: "SMTP_AUTHENTIATE", DefaultSource.SmtpAuthenticate),
                SmtpUsername = configuration.GetString(key: "SMTP_USERNAME", DefaultSource.SmtpUsername),
                SmtpPassword = configuration.GetString(key: "SMTP_PASSWORD", DefaultSource.SmtpPassword),
                S3BucketName = configuration.GetString(key: "S3_BUCKET_NAME", DefaultSource.S3BucketName),
                S3Url = configuration.GetString(key: "S3_URL", DefaultSource.S3Url),
                S3Region = s3Region,
                S3AccessKey = configuration.GetString(key: "S3_ACCESS_KEY", DefaultSource.S3AccessKey),
                S3SecretKey = configuration.GetString(key: "S3_SECRET_KEY", DefaultSource.S3SecretKey),
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
