using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.Auth;

namespace pwr_msi.Controllers {
    [ApiController]
    [Route(template: "api/admin/users")]
    public class AdminUsersController : MsiControllerBase {
        private readonly AppConfig _appConfig;
        private readonly MsiDbContext _dbContext;

        public AdminUsersController(AppConfig appConfig, MsiDbContext dbContext) {
            _appConfig = appConfig;
            _dbContext = dbContext;
        }

        [Route(template: "")]
        public async Task<ActionResult<Page<UserAdminDto>>> List([FromQuery] int page) {
            return await Utils.Paginate(
                queryable: _dbContext.Users.OrderBy(keySelector: u => u.DefaultOrdering()),
                page,
                converter: u => u.AsAdminDto()
            );
        }

        [Route(template: "")]
        [HttpPost]
        public async Task<ActionResult<UserAdminDto>> Create([FromBody] UserAdminDto userAdminDto) {
            var user = userAdminDto.AsNewUser();
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user.AsAdminDto();
        }

        [Route(template: "{id}/")]
        public async Task<ActionResult<UserAdminDto>> Get([FromRoute] int id) {
            var user = await _dbContext.Users.FindAsync(id);
            return user == null ? NotFound() : user.AsAdminDto();
        }

        [Route(template: "{id}/")]
        [HttpPost]
        public async Task<ActionResult<UserAdminDto>> Update([FromRoute] int id, [FromBody] UserAdminDto userAdminDto) {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.UpdateWithAdminDto(userAdminDto);
            await _dbContext.SaveChangesAsync();
            return user.AsAdminDto();
        }
    }
}
