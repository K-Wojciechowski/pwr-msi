using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.AuthPolicies;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.Admin;
using pwr_msi.Models.Dto.Auth;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [Authorize]
    [AdminAuthorize]
    [ApiController]
    [Route(template: "api/admin/users/")]
    public class AdminUsersController : MsiControllerBase {
        private readonly AccountEmailService _accountEmailService;
        private readonly AdminCommonService _adminCommonService;
        private readonly AuthService _authService;
        private readonly MsiDbContext _dbContext;

        public AdminUsersController(
            AccountEmailService accountEmailService,
            AdminCommonService adminCommonService,
            AuthService authService,
            MsiDbContext dbContext
        ) {
            _accountEmailService = accountEmailService;
            _adminCommonService = adminCommonService;
            _authService = authService;
            _dbContext = dbContext;
        }

        [Route(template: "")]
        public async Task<ActionResult<Page<UserAdminDto>>> List([FromQuery] int page = 1) {
            return await Utils.Paginate(
                queryable: _dbContext.Users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ThenBy(u => u.Email),
                page,
                converter: u => u.AsAdminDto()
            );
        }

        [Route(template: "")]
        [HttpPost]
        public async Task<ActionResult<UserAdminDto>> Create([FromBody] UserAdminCreateDto userAdminCreateDto) {
            var user = userAdminCreateDto.AsNewUser();
            user.Password = _authService.HashPassword(user, userAdminCreateDto.Password);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            if (!user.IsVerified) {
                await _accountEmailService.SendVerificationEmail(user);
            }

            return user.AsAdminDto();
        }

        [Route(template: "{id}/")]
        public async Task<ActionResult<UserAdminDto>> Get([FromRoute] int id) {
            var user = await _dbContext.Users.FindAsync(id);
            return user == null ? NotFound() : user.AsAdminDto();
        }

        [Route(template: "{id}/")]
        [HttpPut]
        public async Task<ActionResult<UserAdminDto>> Update([FromRoute] int id, [FromBody] UserAdminDto userAdminDto) {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.UpdateWithAdminDto(userAdminDto);
            await _dbContext.SaveChangesAsync();
            return user.AsAdminDto();
        }

        [Route(template: "{id}/password/")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromRoute] int id, [FromBody] PasswordResetDto resetDto) {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.Password = _authService.HashPassword(user, resetDto.Password);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [Route(template: "{id}/restaurants/")]
        public async Task<ActionResult<List<RestaurantUserDto>>> GetRestaurants([FromRoute] int userId) {
            var query = _dbContext.RestaurantUsers.Where(ru => ru.UserId == userId);
            var ruList = await query.ToListAsync();
            return ruList.Select(ru => ru.AsDto()).ToList();
        }

        [Route(template: "{id}/restaurants/")]
        [HttpPut]
        public async Task<ActionResult<List<RestaurantUserDto>>> UpdateRestaurants([FromRoute] int userId,
            [FromBody] List<RestaurantUserDto> ruDtos) {
            var incomingRestaurantUsers = ruDtos.Where(ru => ru.User.UserId == userId);
            await _adminCommonService.UpdateRestaurantUsers(incomingRestaurantUsers,
                criteria: ru => ru.UserId == userId);
            return await GetRestaurants(userId);
        }

        [Route(template: "typeahead/")]
        public async Task<ActionResult<List<UserAdminDto>>> UsersTypeAhead(string query) {
            var users = _dbContext.Users.Where(r =>
                r.Username.StartsWith(query, StringComparison.OrdinalIgnoreCase) ||
                r.FirstName.StartsWith(query, StringComparison.OrdinalIgnoreCase) ||
                r.LastName.StartsWith(query, StringComparison.OrdinalIgnoreCase)
            );
            return (await users.ToListAsync()).Select(u => u.AsAdminDto()).ToList();
        }
    }
}
