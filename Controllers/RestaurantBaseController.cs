using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var restaurant = await _dbContext.Restaurants.FindAsync(id);
            return restaurant.AsAdminDto();
        }
    }
}
