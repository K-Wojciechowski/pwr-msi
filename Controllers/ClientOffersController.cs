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

namespace pwr_msi.Controllers {
    [ApiController]
    [Authorize]
    [Route(template: "api/offer/restaurants/")]
    public class ClientOffersController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        
        public ClientOffersController(MsiDbContext dbContext) {
            _dbContext = dbContext;
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
            ZonedDateTime now = new ZonedDateTime();
            var userAddress = new GeoCoordinate(last.Latitude, last.Longitude);
            var query = _dbContext.Restaurants.Where(r => r.Name.Contains(name) &&
                                                          r.Cuisines.Any(c => c.Name==cuisine) &&
                                                          r.MenuCategories.Any(mc => mc.Items.Any(mi => mi.Name==meal &&
                                                              (mi.ValidUntil == null || ZonedDateTime.Comparer.Local.Compare((ZonedDateTime)mi.ValidUntil, now) >= 0)
                                                              && ZonedDateTime.Comparer.Local.Compare(now, mi.ValidFrom) > 0)));
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
            ZonedDateTime now = new ZonedDateTime();
            var query = _dbContext.MenuCategories.Where(mc =>( mc.RestaurantId == id)
                                                             && ((mc.ValidUntil == null) || (ZonedDateTime.Comparer.Local.Compare((ZonedDateTime)mc.ValidUntil, now) >= 0))
                                                             && (ZonedDateTime.Comparer.Local.Compare(now, mc.ValidFrom) > 0));
            var mcList = await query.ToListAsync();
            return mcList.Select(mc => mc.AsClientMenuDto()).ToList();
        }
        
    }
}
