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

        [Route(template: "")]
        public async Task<ActionResult<Page<RestaurantAdminDto>>> List([FromQuery] int page = 1) {
            return await Utils.Paginate(
                queryable: _dbContext.Restaurants.OrderBy(u => u.Name).ThenBy(u => u.RestaurantId),
                page,
                converter: r => r.AsAdminDto()
            );
        }

        [Route(template: "")]
        [HttpPost]
        public async Task<ActionResult<RestaurantAdminDto>> Create([FromBody] RestaurantAdminDto restaurantAdminDto) {
            var restaurant = restaurantAdminDto.AsNewRestaurant();
            await _dbContext.Restaurants.AddAsync(restaurant);
            await _dbContext.SaveChangesAsync();
            return restaurant.AsAdminDto();
        }

        [Route(template: "{id}/")]
        public async Task<ActionResult<RestaurantAdminDto>> Get([FromRoute] int id) {
            var restaurant = await _dbContext.Restaurants.FindAsync(id);
            return restaurant == null ? NotFound() : restaurant.AsAdminDto();
        }

        [Route(template: "{id}/")]
        [HttpPost]
        public async Task<ActionResult<RestaurantAdminDto>> Update([FromRoute] int id,
            [FromBody] RestaurantAdminDto restaurantAdminDto) {
            var restaurant = await _dbContext.Restaurants.FindAsync(id);
            if (restaurant == null) return NotFound();
            restaurant.UpdateWithAdminDto(restaurantAdminDto);
            await _dbContext.SaveChangesAsync();
            return restaurant.AsAdminDto();
        }

        [Route(template: "{id}/users/")]
        public async Task<ActionResult<List<RestaurantUserDto>>> GetRestaurants([FromRoute] int restaurantId) {
            var query = _dbContext.RestaurantUsers.Where(ru => ru.RestaurantId == restaurantId);
            var ruList = await query.ToListAsync();
            return ruList.Select(ru => ru.AsDto()).ToList();
        }

        [Route(template: "{id}/users/")]
        [HttpPost]
        public async Task<ActionResult<List<RestaurantUserDto>>> UpdateRestaurants([FromRoute] int restaurantId,
            [FromBody] List<RestaurantUserDto> ruDtos) {
            var incomingRestaurantUsers = ruDtos.Where(ru => ru.Restaurant.RestaruantId == restaurantId);
            await _adminCommonService.UpdateRestaurantUsers(incomingRestaurantUsers,
                criteria: ru => ru.RestaurantId == restaurantId);
            return await GetRestaurants(restaurantId);
        }

        [Route(template: "typeahead/")]
        public async Task<ActionResult<List<RestaurantAdminDto>>> RestaurantsTypeAhead(string query) {
            var restaurants =
                _dbContext.Restaurants.Where(r => r.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase));
            return (await restaurants.ToListAsync()).Select(r => r.AsAdminDto()).ToList();
        }
    }
}
