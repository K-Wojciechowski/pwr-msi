using System.Collections.Generic;
using GeoCoordinatePortable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [ApiController]
    [Authorize]
    [Route(template: "api/offer/restaurants/")]
    public class ClientOffersController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        private readonly MenuService _menuService;
        
        public ClientOffersController(MsiDbContext dbContext, MenuService menuService) {
            _dbContext = dbContext;
            _menuService = menuService;
        }

        private async Task<List<Restaurant>> GetCloseRestaurants(IQueryable<Restaurant> query, GeoCoordinate userLoc) {
            var rList = await query.ToListAsync();
            foreach (var r in rList) {
                var rAddress = new GeoCoordinate(r.Address.Latitude, r.Address.Longitude);
                //value of radius (metres) of area to search restaurants in
                int range = 10000;
                if (rAddress.GetDistanceTo(userLoc) > range) {
                    rList.Remove(r);
                }
            }
            return rList;
        }
        
        [Route(template: "")]
        public async Task<ActionResult<List<RestaurantBasicDto>>> AllRestaurants() {
            var user = await _dbContext.Users.FindAsync(MsiUserId);
            if (user == null) return NotFound();
            Address last = user.Addresses.Last();
            // reverse geosearch using nominatim API
            // Address userAddress = user.Addresses.Last();
            // WebClient wb = new WebClient();
            // wb.Headers.Add("User-Agent: Other");
            // var json = wb.DownloadString("https://nominatim.openstreetmap.org/search?city="
            //                              +userAddress.City+"&street="+userAddress.HouseNumber+" "+userAddress.Street+
            //                              "&postalcode="+userAddress.PostCode+"&format=json");
            var query = _dbContext.Restaurants.AsQueryable();
            var userAddress = new GeoCoordinate(last.Latitude, last.Longitude);
            var rList = await GetCloseRestaurants(query, userAddress);
            return rList.Select(r => r.AsBasicDto()).ToList();
        }
        
        [Route(template: "")]
        public async Task<ActionResult<List<RestaurantBasicDto>>> FilteredRestaurants([FromQuery] string name, [FromQuery] string cuisine, [FromQuery] string meal) {
            var user = await _dbContext.Users.FindAsync(MsiUserId);
            if (user == null) return NotFound();
            Address last = user.Addresses.Last();
            ZonedDateTime now = Utils.Now();
            var likeQueryName = $"%{name}%";
            var likeQueryCuisine = $"%{cuisine}%";
            var likeQueryMeal = $"%{meal}%";
            var query= _dbContext.Restaurants.Where(r => EF.Functions.ILike(r.Name, likeQueryName)&&
                                                         r.Cuisines.Any(c =>  EF.Functions.ILike(c.Name, likeQueryCuisine)) &&
                                                         r.MenuCategories.Any(mc => mc.Items.Any(mi => EF.Functions.ILike(mi.Name, likeQueryMeal) &&
                                                             (mi.ValidUntil == null || ZonedDateTime.Comparer.Local.Compare((ZonedDateTime)mi.ValidUntil, now) >= 0)
                                                             && ZonedDateTime.Comparer.Local.Compare(now, mi.ValidFrom) > 0)));
            var userAddress = new GeoCoordinate(last.Latitude, last.Longitude);
            var rList = await GetCloseRestaurants(query, userAddress);
            return rList.Select(r => r.AsBasicDto()).ToList();
        }

        [Route(template: "{id}/")]
        public async Task<ActionResult<RestaurantDetailDto>> RestaurantDetails([FromRoute] int id) {
            var restaurant = await _dbContext.Restaurants.FindAsync(id);
            return restaurant.AsDetailDto();
        }
        
        [Route(template: "{id}/")]
        public async Task<ActionResult<List<ClientMenuDto>>> RestaurantMenu([FromRoute] int id) {
            var mcList = await _menuService.GetMenuFromDb(id, Utils.Now());
            return mcList.Select(mc => mc.AsClientMenuDto()).ToList();
        }
        
    }
}
