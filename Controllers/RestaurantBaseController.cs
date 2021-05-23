using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models.Dto.Admin;

namespace pwr_msi.Controllers {
    [Authorize]
    [ApiController]
    [Route(template: "api/restaurants/")]
    public class RestaurantBaseController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;

        public RestaurantBaseController(MsiDbContext dbContext) {
            _dbContext = dbContext;
        }

        [Route(template: "{id}/")]
        public async Task<ActionResult<RestaurantAdminDto>> GetRestaurantInfo(int id) {
            var restaurant = await _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Cuisines)
                .Where(r => r.RestaurantId == id)
                .FirstOrDefaultAsync();
            return restaurant.AsAdminDto();
        }
    }
}
