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
    public class DeliveryController : OrdersControllerBase {
        private readonly OrderTaskService _orderTaskService;
        protected override bool IncludeDeliveryPerson => true;

        public DeliveryController(MsiDbContext dbContext, OrderTaskService orderTaskService,
            OrderDetailsService orderDetailsService) {
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

            return await GetBasicOrders(query);
        }

        [Route(template: "waiting/")]
        public async Task<ActionResult<List<OrderBasicDto>>> AllWaitingOrders([FromQuery] float lat,
            [FromQuery] float lng, [FromQuery] int range, [FromQuery] int restaurantId) {
            var restaurants = await _dbContext.RestaurantUsers
                .Where(ru => ru.UserId == MsiUserId && ru.CanDeliverOrders)
                .Select(ru => ru.RestaurantId).ToListAsync();
            var query = _dbContext.Orders
                .OrderByDescending(o => o.Updated)
                .Where(o => o.DeliveryPersonId == null)
                .Where(o => restaurants.Contains(o.RestaurantId))
                .Where(o =>
                    o.Status == OrderStatus.PREPARED || o.Status == OrderStatus.ACCEPTED);

            if (range == 0) {
                return await GetBasicOrders(query);
            }

            var enrichedQuery = query.Include(o => o.Restaurant).Include("Restaurant.Address");
            var userAddress = new GeoCoordinate(lat, lng);
            var orders = await GetNearbyOrders(enrichedQuery, userAddress, range);
            return await GetBasicOrders(orders);
        }

        [Route(template: "history/")]
        public async Task<ActionResult<List<OrderBasicDto>>> AllCompletedOrders([FromQuery] int restaurant = -1) {
            var query = _dbContext.Orders.Where(o => o.DeliveryPersonId == MsiUserId);
            if (restaurant > 0) {
                query = query.Where(o => o.RestaurantId == restaurant);
            }

            return await GetBasicOrders(query);
        }

        [Route(template: "active/{id}/complete/")]
        [HttpPost]
        public async Task<ActionResult<OrderBasicDto?>> MarkAsDelivered([FromRoute] int id) {
            var orderQuery = _dbContext.Orders
                .Where(o => o.OrderId == id)
                .Where(o => o.DeliveryPersonId == MsiUserId)
                .Where(o => o.Status == OrderStatus.PREPARED || o.Status == OrderStatus.ACCEPTED);
            var order = await orderQuery.FirstOrDefaultAsync();

            if (order == null) return NotFound();
            await _orderTaskService.TryCompleteTask(order, OrderTaskType.DELIVER, MsiUser);
            return await _orderDetailsService.GetBasicOrderById(order.OrderId, IncludeDeliveryPerson);
        }

        [Route(template: "waiting/{id}/assign/")]
        [HttpPost]
        public async Task<ActionResult<OrderBasicDto?>> AssignToSelf([FromRoute] int id) {
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
            var res = await _orderTaskService.TryRegisterOrAssignTask(order, OrderTaskType.DELIVER, AssigneeType.DELIVERY,
                assigneeRestaurantId: null, assigneeUserId: MsiUserId);
            if (res) {
                order.DeliveryPersonId = MsiUserId;
                order.Updated = Utils.Now();
            }

            await _dbContext.SaveChangesAsync();
            return await _orderDetailsService.GetBasicOrderById(order.OrderId, IncludeDeliveryPerson);
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
                .Where(o => o.DeliveryPersonId == MsiUserId || o.Status == OrderStatus.PREPARED ||
                            o.Status == OrderStatus.ACCEPTED);

            var order = await orderQuery.FirstOrDefaultAsync();

            if (order == null) return NotFound();
            return await _orderDetailsService.GetOrderById(order.OrderId, includeDeliveryPerson: true);
        }
    }
}
