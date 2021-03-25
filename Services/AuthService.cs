using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using pwr_msi.Models;

namespace pwr_msi.Services {
    public class AuthService {
        public const string ClaimUserId = "userId";
        public const string ClaimRefreshKey = "refresh";
        public const string ClaimRefreshValue = "refresh";

        private AppConfig _appConfig;

        public AuthService(AppConfig appConfig) {
            _appConfig = appConfig;
        }

        public JwtSecurityToken ReadToken(string tokenStr) {
            try {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(tokenStr, _appConfig.JwtValidationParameters, out var token);
                return (JwtSecurityToken) token;
            } catch {
                return null;
            }
        }

        public Claim GetUserIdClaim(int userId) => new(ClaimUserId, userId.ToString());
        public Claim GetRefreshClaim() => new (ClaimRefreshKey, ClaimRefreshValue);

        public int? ExtractUserFromToken(JwtSecurityToken token) {
            var userIdClaim = token?.Claims?.FirstOrDefault(c => c.Type == ClaimUserId);
            return userIdClaim == null ? null : Utils.TryParseInt(userIdClaim.Value);
        }


        public bool IsRefreshToken(JwtSecurityToken token) {
            var refreshClaim = token?.Claims?.FirstOrDefault(c => c.Type == ClaimRefreshKey);
            return refreshClaim != null && refreshClaim.Value == ClaimRefreshValue;
        }

        public string GenerateJwtToken(User user, TimeSpan lifeTime, IReadOnlyCollection<Claim> extraClaims = null) =>
            GenerateJwtToken(user.UserId, lifeTime, extraClaims);

        public string GenerateJwtToken(int userId, TimeSpan lifeTime, IReadOnlyCollection<Claim> extraClaims = null) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var baseClaims = new List<Claim> {GetUserIdClaim(userId)};
            var allClaims = extraClaims == null ? baseClaims.ToArray() : baseClaims.Concat(extraClaims).ToArray();
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(allClaims),
                Expires = DateTime.UtcNow.Add(lifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_appConfig.JwtKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
}
    }
