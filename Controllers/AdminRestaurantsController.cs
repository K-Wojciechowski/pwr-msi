using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.Admin;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [Authorize]
    [AdminAuthorize]
    [ApiController]
    [Route(template: "api/admin/restaurants/")]
    public class AdminRestaurantsController : MsiControllerBase {
        private readonly AdminCommonService _adminCommonService;
        private readonly MsiDbContext _dbContext;

        public AdminRestaurantsController(MsiDbContext dbContext, AdminCommonService adminCommonService) {
            _dbContext = dbContext;
            _adminCommonService = adminCommonService;
        }

        private IQueryable<Restaurant> GetRestaurantsBaseQueryable() {
            return _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Cuisines);
        }

        private IQueryable<RestaurantUser> GetRestaurantUsersBaseQueryable() {
            return _dbContext.RestaurantUsers
                .Include(ru => ru.Restaurant)
                .Include(ru => ru.User);
        }

        [Route(template: "")]
        public async Task<ActionResult<Page<RestaurantFullDto>>> List([FromQuery] int page = 1) {
            return await Utils.Paginate(
                queryable: GetRestaurantsBaseQueryable().OrderBy(r => r.Name).ThenBy(r => r.RestaurantId),
                page,
                converter: r => r.AsAdminDto()
            );
        }

        [Route(template: "")]
        [HttpPost]
        public async Task<ActionResult<RestaurantFullDto>> Create([FromBody] RestaurantFullDto restaurantFullDto) {
            var restaurant = restaurantFullDto.AsNewRestaurant();
            var cuisineIds = restaurantFullDto.Cuisines.Select(c => c.CuisineId);
            restaurant.Cuisines = await _dbContext.Cuisines.Where(c => cuisineIds.Contains(c.CuisineId)).ToListAsync();
            await _dbContext.Restaurants.AddAsync(restaurant);
            await _dbContext.SaveChangesAsync();
            return restaurant.AsAdminDto();
        }

        [Route(template: "{id}/")]
        public async Task<ActionResult<RestaurantFullDto>> Get([FromRoute] int id) {
            var restaurant = await GetRestaurantsBaseQueryable().Where(r => r.RestaurantId == id).FirstOrDefaultAsync();
            return restaurant == null ? NotFound() : restaurant.AsAdminDto();
        }

        [Route(template: "{id}/")]
        [HttpPut]
        public async Task<ActionResult<RestaurantFullDto>> Update([FromRoute] int id,
            [FromBody] RestaurantFullDto restaurantFullDto) {
            var restaurant = await GetRestaurantsBaseQueryable().Where(r => r.RestaurantId == id).FirstOrDefaultAsync();
            if (restaurant == null) return NotFound();
            await restaurant.UpdateWithAdminDto(restaurantFullDto, _dbContext.Cuisines);
            await _dbContext.SaveChangesAsync();
            return restaurant.AsAdminDto();
        }

        [Route(template: "{id}/users/")]
        public async Task<ActionResult<List<RestaurantUserDto>>> GetUsers([FromRoute] int id) {
            var query = GetRestaurantUsersBaseQueryable().Where(ru => ru.RestaurantId == id);
            var ruList = await query.ToListAsync();
            return ruList.Select(ru => ru.AsDto()).ToList();
        }

        [Route(template: "{id}/users/")]
        [HttpPut]
        public async Task<ActionResult<List<RestaurantUserDto>>> UpdateUsers([FromRoute] int id,
            [FromBody] List<RestaurantUserDto> ruDtos) {
            var incomingRestaurantUsers = ruDtos.Where(ru => ru.Restaurant.RestaurantId == id);
            await _adminCommonService.UpdateRestaurantUsers(incomingRestaurantUsers,
                criteria: ru => ru.RestaurantId == id);
            return await GetUsers(id);
        }

        [Route(template: "typeahead/")]
        public async Task<ActionResult<List<RestaurantFullDto>>> RestaurantsTypeAhead(
            [FromQuery(Name = "q")] string query) {
            var likeQuery = $"{query}%";
            var restaurants =
                GetRestaurantsBaseQueryable().Where(r => EF.Functions.ILike(r.Name, likeQuery));
            return (await restaurants.ToListAsync()).Select(r => r.AsAdminDto()).ToList();
        }
    }
}
