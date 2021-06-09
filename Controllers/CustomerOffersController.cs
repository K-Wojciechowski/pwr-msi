using System;
using System.Collections.Generic;
using GeoCoordinatePortable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.RestaurantMenu;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [ApiController]
    [Authorize]
    [Route(template: "api/offer/restaurants/")]
    public class CustomerOffersController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        private readonly MenuService _menuService;

        public CustomerOffersController(MsiDbContext dbContext, MenuService menuService) {
            _dbContext = dbContext;
            _menuService = menuService;
        }

        private static async Task<List<Restaurant>> GetCloseRestaurants(IQueryable<Restaurant> query,
            GeoCoordinate userLoc) {
            var rList = await query.ToListAsync();
            return rList.Where(r => {
                // This could be computed in Postgres, but that might be difficult, so let's do it here.
                var rAddress = new GeoCoordinate(r.Address.Latitude, r.Address.Longitude);
                //value of radius (metres) of area to search restaurants in
                return rAddress.GetDistanceTo(userLoc) <= Constants.RestaurantSearchRange;
            }).ToList();
        }

        private IQueryable<Restaurant> RestaurantsDetailQuery() =>
            _dbContext.Restaurants.Include(r => r.Address).Include(r => r.Cuisines);

        [Route(template: "")]
        public async Task<ActionResult<Page<RestaurantDetailDto>>> GetRestaurants([FromQuery] string search = "",
            [FromQuery] string cuisines = "", [FromQuery] double latitude = 0, [FromQuery] double longitude = 0,
            [FromQuery] int page = 1) {
            var query = RestaurantsDetailQuery();
            if (cuisines != null && cuisines.Length > 0) {
                var cuisinesIds = cuisines.Split(',').Select(c => {
                    var status = int.TryParse(c, out var res);
                    return status ? res : -1;
                });
                foreach (var id in cuisinesIds) {
                    Console.WriteLine(id);
                }

                Console.WriteLine("restauracje " + query.ToList().Count);
                query = query.Where(r => r.Cuisines.Any(c => cuisinesIds.Contains(c.CuisineId)));
                Console.WriteLine("Kuchnia "+(await _dbContext.Cuisines.FindAsync(cuisinesIds.ToList()[0])).Name);
                Console.WriteLine("restauracje " + query.ToList().Count);

            }

            if (search != null && search.Length > 0) {
                var now = Utils.Now();
                var matchPattern = $"%{search.Trim()}%";
                query = query.Include(r => r.MenuItems).Where(r =>
                    EF.Functions.ILike(r.Name, matchPattern) ||
                    r.MenuItems.Any(mi => EF.Functions.ILike(mi.Name, matchPattern)
                                          && (mi.ValidUntil == null ||
                                              ZonedDateTime.Comparer.Local.Compare(
                                                  (ZonedDateTime) mi.ValidUntil, now) >= 0)
                                          && ZonedDateTime.Comparer.Local.Compare(now,
                                              mi.ValidFrom) > 0));
            }

            var userLocation = new GeoCoordinate(latitude, longitude);
            List<Restaurant> restaurants;
            if (latitude == 0 && longitude == 0) {
                restaurants = await query.ToListAsync();
            } else {
                restaurants = await GetCloseRestaurants(query, userLocation);
            }

            return Utils.PaginateList(restaurants, page, r => r.AsDetailDto());
        }

        [Route(template: "{id}/")]
        public async Task<ActionResult<RestaurantDetailDto>> RestaurantDetails([FromRoute] int id) {
            var restaurant = await RestaurantsDetailQuery().Where(r => r.RestaurantId == id).FirstAsync();
            return restaurant.AsDetailDto();
        }

        [Route(template: "{id}/menu/")]
        public async Task<ActionResult<List<MenuCategoryWithItemsDto>>> RestaurantMenu([FromRoute] int id) {
            return await _menuService.GetMenuFromCache(id, Utils.Now());
        }

        [Route("cuisines/")]
        public async Task<ActionResult<List<Cuisine>>> GetCuisines() {
            return await _dbContext.Cuisines.OrderBy(c => c.Name).ToListAsync();
        }
    }
}
