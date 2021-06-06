#nullable enable
using System.Collections.Generic;
using GeoCoordinatePortable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Enum;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [Authorize]
    [ApiController]
    [Route(template: "api/delivery/")]
    public class DeliveryController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        private readonly OrderTaskService _orderTaskService;
        private readonly OrderDetailsService _orderDetailsService;

        public DeliveryController(MsiDbContext dbContext, OrderTaskService orderTaskService, OrderDetailsService orderDetailsService) {
            _dbContext = dbContext;
            _orderTaskService = orderTaskService;
            _orderDetailsService = orderDetailsService;
        }

        private async Task<List<Order>> GetNearbyOrders(IQueryable<Order> query, GeoCoordinate userLoc, int range) {
            var oList = await query.ToListAsync();
            var nearby = new List<Order>();
            foreach (var o in oList) {
                var rAddress = new GeoCoordinate(o.Restaurant.Address.Latitude, o.Restaurant.Address.Longitude);
                if (rAddress.GetDistanceTo(userLoc) <= range) {
                    nearby.Add(o);
                }
            }

            return nearby;
        }

        [Route(template: "active/")]
        public async Task<ActionResult<List<OrderBasicDto>>> AllActiveOrders([FromQuery] int restaurant = -1) {
            var query = _dbContext.Orders.Where(o => o.DeliveryPersonId == MsiUserId)
                .Where(o =>
                    o.Status == OrderStatus.PREPARED || o.Status == OrderStatus.ACCEPTED);
            if (restaurant > 0) {
                query = query.Where(o => o.RestaurantId == restaurant);
            }

            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }

        [Route(template: "waiting/")]
        public async Task<ActionResult<List<OrderBasicDto>>> AllWaitingOrders([FromQuery] float lat,
            [FromQuery] float lng, [FromQuery] int range, [FromQuery] int restaurantId) {
            var restaurants = await _dbContext.RestaurantUsers
                .Where(ru => ru.UserId == MsiUserId && ru.CanDeliverOrders)
                .Select(ru => ru.RestaurantId).ToListAsync();
            var query = _dbContext.Orders.Where(o => o.DeliveryPersonId == null)
                .Where(o => restaurants.Contains(o.RestaurantId))
                .Where(o =>
                    o.Status == OrderStatus.PREPARED || o.Status == OrderStatus.ACCEPTED);

            List<Order> orders;
            if (range == 0) {
                orders = await query.ToListAsync();
            } else {
                var enrichedQuery = query.Include(o => o.Restaurant).Include("Restaurant.Address");
                var userAddress = new GeoCoordinate(lat, lng);
                orders = await GetNearbyOrders(enrichedQuery, userAddress, range);
            }

            return orders.Select(o => o.AsBasicDto()).ToList();
        }

        [Route(template: "history/")]
        public async Task<ActionResult<List<OrderBasicDto>>> AllCompletedOrders([FromQuery] int restaurant = -1) {
            var query = _dbContext.Orders.Where(o => o.DeliveryPersonId == MsiUserId);
            if (restaurant > 0) {
                query = query.Where(o => o.RestaurantId == restaurant);
            }

            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }

        [Route(template: "active/{id}/complete/")]
        [HttpPost]
        public async Task<ActionResult<OrderBasicDto>> MarkAsDelivered([FromRoute] int id) {
            var orderQuery = _dbContext.Orders
                .Where(o => o.OrderId == id)
                .Where(o => o.DeliveryPersonId == MsiUserId)
                .Where(o => o.Status == OrderStatus.PREPARED || o.Status == OrderStatus.ACCEPTED);
            var order = await orderQuery.FirstOrDefaultAsync();

            if (order == null) return NotFound();
            await _orderTaskService.TryCompleteTask(order, OrderTaskType.DELIVER, MsiUser);
            return order.AsBasicDto();
        }

        [Route(template: "waiting/{id}/assign/")]
        [HttpPost]
        public async Task<ActionResult<OrderBasicDto>> AssignToSelf([FromRoute] int id) {
            var restaurants = await _dbContext.RestaurantUsers
                .Where(ru => ru.UserId == MsiUserId && ru.CanDeliverOrders)
                .Select(ru => ru.RestaurantId).ToListAsync();

            var orderQuery = _dbContext.Orders
                .Where(o => o.OrderId == id)
                .Where(o => o.DeliveryPersonId == null)
                .Where(o => restaurants.Contains(o.RestaurantId))
                .Where(o => o.Status == OrderStatus.PREPARED || o.Status == OrderStatus.ACCEPTED);
            var order = await orderQuery.FirstOrDefaultAsync();

            if (order == null) return NotFound();
            order.DeliveryPersonId = MsiUserId;

            // TODO register/assign order task

            await _dbContext.SaveChangesAsync();
            return order.AsBasicDto();
        }

        [Route("order/{id}/")]
        public async Task<ActionResult<OrderDto?>> OrderDetails([FromRoute] int id) {
            var restaurants = await _dbContext.RestaurantUsers
                .Where(ru => ru.UserId == MsiUserId && ru.CanDeliverOrders)
                .Select(ru => ru.RestaurantId).ToListAsync();

            var orderQuery = _dbContext.Orders
                .Where(o => o.OrderId == id)
                .Where(o => o.DeliveryPersonId == null || o.DeliveryPersonId == MsiUserId)
                .Where(o => restaurants.Contains(o.RestaurantId))
                .Where(o => o.DeliveryPersonId == MsiUserId || o.Status == OrderStatus.PREPARED || o.Status == OrderStatus.ACCEPTED);

            var order = await orderQuery.FirstOrDefaultAsync();

            if (order == null) return NotFound();
            return await _orderDetailsService.GetOrderById(order.OrderId, includeDeliveryPerson: true);
        }
    }
}
