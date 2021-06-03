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

        private static async Task<List<Restaurant>> GetCloseRestaurants(IQueryable<Restaurant> query, GeoCoordinate userLoc) {
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
        public async Task<ActionResult<Page<RestaurantDetailDto>>> CloseRestaurants([FromQuery] string lng ="", [FromQuery] string lat ="", [FromQuery] int page = 1) {
            var user = await _dbContext.Users.FindAsync(MsiUserId);
            if (user == null) return NotFound();
            // TODO use coords from browser?
            Address last = user.Addresses.Last();
            // reverse geosearch using nominatim API
            // Address userAddress = user.Addresses.Last();
            // WebClient wb = new WebClient();
            // wb.Headers.Add("User-Agent: Other");
            // var json = wb.DownloadString("https://nominatim.openstreetmap.org/search?city="
            //                              +userAddress.City+"&street="+userAddress.HouseNumber+" "+userAddress.Street+
            //                              "&postalcode="+userAddress.PostCode+"&format=json");
            var query = RestaurantsDetailQuery();
            var userAddress = new GeoCoordinate(last.Latitude, last.Longitude);
            var rList = await GetCloseRestaurants(query, userAddress);
            return await Utils.Paginate(rList.AsQueryable(), page, r => r.AsDetailDto());
        }
        
        private IQueryable<Restaurant> GetRestaurantsBaseQueryable() {
            return _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Cuisines);
        }
        
        [Route(template: "all/")]
        public async Task<ActionResult<Page<RestaurantDetailDto>>> AllRestaurants([FromQuery] int page = 1) {
            return await Utils.Paginate(
                queryable: GetRestaurantsBaseQueryable().OrderBy(r => r.Name).ThenBy(r => r.RestaurantId),
                page,
                converter: r => r.AsDetailDto()
            );
        }

        [Route(template: "search/")]
        public async Task<ActionResult<Page<RestaurantDetailDto>>> FilteredRestaurants([FromQuery] string phrase, [FromQuery] int page = 1) {
            var user = await _dbContext.Users.FindAsync(MsiUserId);
            if (user == null) return NotFound();
            //Address last = user.Addresses.Last();
            ZonedDateTime now = Utils.Now();
            var likeQuery = $"%{phrase}%";
            var query = _dbContext.Restaurants.Where(r => r.Name.Contains(phrase) 
                                                          || r.Cuisines.Any(c => c.Name.Contains(phrase) 
                                                              || r.MenuItems.Any(mi => mi.Name.Contains(phrase)
                                                                  && (mi.ValidUntil == null ||
                                                                      ZonedDateTime.Comparer.Local.Compare(
                                                                          (ZonedDateTime) mi.ValidUntil, now) >= 0) 
                                                                    && ZonedDateTime.Comparer.Local.Compare(now, mi.ValidFrom) > 0)));
            // var query = RestaurantsDetailQuery()
            //     .Where(r => EF.Functions.ILike(r.Name, likeQuery))
            //     .Where(r => r.Cuisines.Any(c =>
            //         EF.Functions.ILike(c.Name, likeQuery)))
            //     .Where(r => r.MenuItems.Any(mi =>
            //         EF.Functions.ILike(mi.Name, likeQuery)
            //         &&
            //         (mi.ValidUntil == null ||
            //          ZonedDateTime.Comparer.Local.Compare(
            //              (ZonedDateTime) mi.ValidUntil, now) >= 0)
            //         && ZonedDateTime.Comparer.Local.Compare(now,
            //             mi.ValidFrom) > 0));
            //var userAddress = new GeoCoordinate(last.Latitude, last.Longitude);
            //var rList = await GetCloseRestaurants(query, userAddress);
            return await Utils.Paginate(query, page, r => r.AsDetailDto());
        }

        [Route(template: "{id}/")]
        public async Task<ActionResult<RestaurantDetailDto>> RestaurantDetails([FromRoute] int id) {
            var restaurant = await _dbContext.Restaurants.FindAsync(id);
            return restaurant.AsDetailDto();
        }

        [Route(template: "{id}/menu/")]
        public async Task<ActionResult<List<MenuCategoryWithItemsDto>>> RestaurantMenu([FromRoute] int id) {
            return await _menuService.GetMenuFromCache(id, Utils.Now());
        }
    }
}
