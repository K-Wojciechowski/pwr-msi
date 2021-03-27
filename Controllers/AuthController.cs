using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class AuthController : MsiControllerBase {
        private readonly AppConfig _appConfig;
        private readonly MsiDbContext _dbContext;
        private readonly AuthService _authService;

        public AuthController(AppConfig appConfig, MsiDbContext dbContext, AuthService authService) {
            _appConfig = appConfig;
            _dbContext = dbContext;
            _authService = authService;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<AuthResult>> Authenticate([FromBody] AuthInput authInput) {
            try {
                var user = await _dbContext.Users.SingleAsync(u => u.Username == authInput.Username);
                var result = _authService.VerifyHashedPassword(user, authInput.Password);

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (result == PasswordVerificationResult.Failed) {
                    return BadRequest();
                }

                if (result == PasswordVerificationResult.SuccessRehashNeeded) {
                    user.Password = _authService.HashPassword(user, authInput.Password);
                    await _dbContext.SaveChangesAsync();
                }

                var authToken = _authService.GenerateJwtToken(user, _appConfig.AuthTokenLifetime);
                var refreshToken = _authService.GenerateJwtToken(user, _appConfig.RefreshTokenLifetime,
                    new List<Claim> {_authService.GetRefreshClaim()});
                return new AuthResult {
                    AuthToken = authToken,
                    RefreshIn = (int)_appConfig.AuthTokenLifetime.TotalSeconds,
                    RefreshToken = refreshToken,
                };
            } catch (InvalidOperationException) {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("refresh/")]
        public ActionResult<RefreshResult> RefreshToken([FromBody] RefreshInput refreshInput) {
            var tokenStr = refreshInput.RefreshToken;
            var token = _authService.ReadToken(tokenStr);
            var userId = _authService.ExtractUserFromToken(token);
            // ReSharper disable once InvertIf
            if (_authService.IsRefreshToken(token) && userId != null) {
                var authToken = _authService.GenerateJwtToken(userId.Value, _appConfig.AuthTokenLifetime);
                return new RefreshResult {AuthToken = authToken, RefreshIn = (int)_appConfig.AuthTokenLifetime.TotalSeconds};
            }

            return BadRequest();
        }

        [Route("testcreate/")]
        public async Task<ActionResult> TestCreate() {
            var addr = new Address {
                Addressee = "T T",
                City = "C",
                PostCode = "PC",
                Country = "PL",
                FirstLine = "X",
                SecondLine = "Y"
            };
            var user = new User {Email = "test@test.pl", Username = "test", FirstName = "Test", LastName = "User", BillingAddress = addr};
            user.Password = _authService.HashPassword(user, "test");
            await _dbContext.AddAsync(addr);
            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return Created("hello", "world");
        }

        [Authorize]
        [Route("whoami/")]
        public ActionResult<string> WhoAmI() {
            return "Hello, " + MsiUser.FullName;
        }
    }
}
