using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pwr_msi.Models;
using pwr_msi.Models.Dto.Auth;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [ApiController]
    [Route(template: "api/auth/")]
    public class AuthController : MsiControllerBase {
        private readonly AccountEmailService _accountEmailService;
        private readonly AppConfig _appConfig;
        private readonly AuthService _authService;
        private readonly MsiDbContext _dbContext;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppConfig appConfig, MsiDbContext dbContext, AuthService authService,
            AccountEmailService accountEmailService, ILogger<AuthController> logger) {
            _appConfig = appConfig;
            _dbContext = dbContext;
            _authService = authService;
            _accountEmailService = accountEmailService;
            _logger = logger;
        }

        [HttpPost]
        [Route(template: "")]
        public async Task<ActionResult<AuthResult>> Authenticate([FromBody] AuthInput authInput) {
            try {
                var user = await _dbContext.Users.SingleAsync(predicate: u =>
                    u.Username == authInput.Username && u.IsVerified && u.IsActive);
                var result = _authService.VerifyHashedPassword(user, authInput.Password);

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (result == PasswordVerificationResult.Failed) return BadRequest();

                if (result == PasswordVerificationResult.SuccessRehashNeeded) {
                    user.Password = _authService.HashPassword(user, authInput.Password);
                    await _dbContext.SaveChangesAsync();
                }

                var authToken = _authService.GenerateJwtToken(user, _appConfig.AuthTokenLifetime);
                var refreshToken = _authService.GenerateJwtToken(user, _appConfig.RefreshTokenLifetime,
                    extraClaims: new List<Claim> {_authService.GetRefreshClaim()});
                return new AuthResult {
                    AuthToken = authToken,
                    RefreshAt = _authService.GetExpiryDate(authToken),
                    RefreshToken = refreshToken,
                };
            } catch (InvalidOperationException e) {
                _logger.LogDebug(e.StackTrace);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route(template: "refresh/")]
        public ActionResult<RefreshResult> RefreshToken([FromBody] RefreshInput refreshInput) {
            var tokenStr = refreshInput.RefreshToken;
            var token = _authService.ReadToken(tokenStr);
            var userId = _authService.ExtractUserFromToken(token);
            // ReSharper disable once InvertIf
            if (_authService.IsRefreshToken(token) && userId != null) {
                var authToken = _authService.GenerateJwtToken(userId.Value, _appConfig.AuthTokenLifetime);
                return new RefreshResult {AuthToken = authToken, RefreshAt = _authService.GetExpiryDate(authToken)};
            }

            return BadRequest();
        }

        [HttpPost]
        [Route(template: "register/")]
        public async Task<ActionResult<UserProfileDto>> Register([FromBody] NewUserDto newUser) {
            var user = new User {
                Username = newUser.Username,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                IsActive = true,
                IsAdmin = false,
                IsVerified = false,
                Balance = 0,
                BillingAddress = newUser.BillingAddress,
            };
            if (user.BillingAddress != null) {
                user.BillingAddress.AddressId = 0;
                user.BillingAddress.Users = new List<User> {user};
            }

            user.Password = _authService.HashPassword(user, newUser.Password);

            if (!TryValidateModel(User, prefix: nameof(User))) return BadRequest();

            // Check if an admin is present, and set the account to admin if not.
            var anyAdminUser = await _dbContext.Users.FirstOrDefaultAsync(predicate: u => u.IsAdmin);
            if (anyAdminUser == null) user.IsAdmin = true;

            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            await _accountEmailService.SendVerificationEmail(user);
            return StatusCode(statusCode: (int) HttpStatusCode.Created, value: user.AsProfile());
        }

        [Authorize]
        [Route(template: "access/")]
        public async Task<ActionResult<UserAccessDto>> GetAccess() {
            var userPermissions = _dbContext.RestaurantUsers.Include(ru => ru.Restaurant).Where(predicate: ru => ru.UserId == MsiUserId);
            var manage = await userPermissions.Where(predicate: ru => ru.CanManage).ToListAsync();
            var accept = await userPermissions.Where(predicate: ru => ru.CanAcceptOrders).ToListAsync();
            var deliver = await userPermissions.Where(predicate: ru => ru.CanDeliverOrders).ToListAsync();
            return new UserAccessDto {
                Profile = MsiUser.AsProfile(),
                Admin = MsiUser.IsAdmin,
                Manage = manage.Select(selector: ru => ru.Restaurant.AsBasicDto()),
                Accept = accept.Select(selector: ru => ru.Restaurant.AsBasicDto()),
                Deliver = deliver.Select(selector: ru => ru.Restaurant.AsBasicDto()),
            };
        }

        [Authorize]
        [Route(template: "profile/")]
        public ActionResult<UserProfileDto> GetProfile() {
            return MsiUser.AsProfile();
        }
        
        [Authorize]
        [Route(template: "profile/edit/")]
        [HttpPut]
        public async Task<ActionResult<UserProfileDto>> ModifyProfile([FromBody] UserProfileDto dto) {
            MsiUser.UpdateWithProfileDto(dto);
            await _dbContext.SaveChangesAsync();
            return MsiUser.AsProfile();
        }
        
        [Authorize]
        [Route(template: "profile/")]
        [HttpPut]
        public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] EditProfileDto changes) {
            var user = MsiUser;
            if (!string.IsNullOrEmpty(changes.Password))
                user.Password = _authService.HashPassword(user, changes.Password);

            if (changes.Email != user.Email) {
                user.Email = changes.Email;
                user.IsVerified = false;
                await _accountEmailService.SendVerificationEmail(user);
            }

            user.FirstName = changes.FirstName;
            user.LastName = changes.LastName;
            if (user.BillingAddressId == null) {
                user.BillingAddress = changes.BillingAddress;
                user.BillingAddress.Users = new List<User> {user};
            } else {
                user.BillingAddress = changes.BillingAddress;
            }

            await _dbContext.SaveChangesAsync();
            return user.AsProfile();
        }

        [Route(template: "forgot/")]
        [HttpPost]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto source) {
            var user = await _dbContext.Users.FirstOrDefaultAsync(predicate: u => u.Email == source.Email);
            if (user != null) {
                if (user.IsVerified)
                    await _accountEmailService.SendResetEmail(user);
                else
                    await _accountEmailService.SendVerificationEmail(user);
            }

            return Ok();
        }

        [HttpPost]
        [Route(template: "verify/{token}/")]
        public async Task<ActionResult> VerifyEmail([FromRoute] string token) {
            var verificationToken =
                await _dbContext.VerificationTokens.Include(t => t.User).Where(t => t.Token == token).FirstOrDefaultAsync();
            if (verificationToken == null) return NotFound();

            if (!verificationToken.IsValid) return BadRequest();

            verificationToken.User.IsVerified = true;
            verificationToken.IsUsed = true;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [Route(template: "reset/{token}/")]
        public async Task<ActionResult> StartReset([FromRoute] string token) {
            var verificationToken =
                await _dbContext.VerificationTokens.Where(t => t.Token == token).FirstOrDefaultAsync();
            if (verificationToken == null) return NotFound();

            return verificationToken.IsValid ? Ok() : BadRequest();
        }

        [HttpPost]
        [Route(template: "reset/{token}/")]
        public async Task<ActionResult> VerifyEmail([FromRoute] string token, [FromBody] PasswordResetDto resetDto) {
            var verificationToken =
                await _dbContext.VerificationTokens.Include(t => t.User).Where(t => t.Token == token).FirstOrDefaultAsync();
            if (verificationToken == null) return NotFound();

            if (!verificationToken.IsValid) return BadRequest();

            verificationToken.User.Password = _authService.HashPassword(verificationToken.User, resetDto.Password);
            verificationToken.IsUsed = true;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
