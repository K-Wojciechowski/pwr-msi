using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase {
        private readonly AppConfig _appConfig;
        private readonly MsiDbContext _dbContext;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly AuthService _authService;

        public AuthController(AppConfig appConfig, MsiDbContext dbContext, PasswordHasher<User> passwordHasher,
            AuthService authService) {
            _appConfig = appConfig;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<AuthResult>> Authenticate([FromBody] AuthInput authInput) {
            try {
                var user = await _dbContext.Users.SingleAsync(u => u.Username == authInput.Username);
                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, authInput.Password);

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (result == PasswordVerificationResult.Failed) {
                    return BadRequest();
                }

                if (result == PasswordVerificationResult.SuccessRehashNeeded) {
                    user.Password = _passwordHasher.HashPassword(user, authInput.Password);
                    await _dbContext.SaveChangesAsync();
                }

                var authToken = _authService.GenerateJwtToken(user, _appConfig.AuthTokenLifetime);
                var refreshToken = _authService.GenerateJwtToken(user, _appConfig.RefreshTokenLifetime,
                    new List<Claim> {_authService.GetRefreshClaim()});
                return new AuthResult {
                    AuthToken = authToken,
                    RefreshIn = _appConfig.AuthTokenLifetime.Seconds,
                    RefreshToken = refreshToken,
                };
            } catch (InvalidOperationException) {
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult<RefreshResult> RefreshToken([FromBody] RefreshInput refreshInput) {
            var tokenStr = refreshInput.RefreshToken;
            var token = _authService.ReadToken(tokenStr);
            var userId = _authService.ExtractUserFromToken(token);
            // ReSharper disable once InvertIf
            if (_authService.IsRefreshToken(token) && userId != null) {
                var authToken = _authService.GenerateJwtToken(userId.Value, _appConfig.AuthTokenLifetime);
                return new RefreshResult {AuthToken = authToken, RefreshIn = _appConfig.AuthTokenLifetime.Seconds};
            }

            return BadRequest();
        }
    }
}
