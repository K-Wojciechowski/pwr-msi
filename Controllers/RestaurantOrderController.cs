#nullable enable
using System;
using System.Collections.Generic;
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
    [Route(template: "api/restaurants/")]
    public class RestaurantOrderController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        private readonly OrderDetailsService _orderDetailsService;
        private readonly OrderTaskService _orderTaskService;

        public RestaurantOrderController(MsiDbContext dbContext, OrderDetailsService orderDetailsService,
            OrderTaskService orderTaskService) {
            _dbContext = dbContext;
            _orderDetailsService = orderDetailsService;
            _orderTaskService = orderTaskService;
        }

        private IQueryable<Order> OrderBasicQuery() {
            return _dbContext.Orders.Include(o => o.Customer).Include(o => o.Restaurant).Include(o => o.Address);
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/")]
        public async Task<ActionResult<List<OrderBasicDto>>> GetAllOrders([FromRoute] int id) {
            var query = OrderBasicQuery().Where(o => o.RestaurantId == id);
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/")]
        public async Task<ActionResult<List<OrderBasicDto>>> GetOrders([FromRoute] int id, [FromQuery] string status) {
            if (!Enum.TryParse(status.ToUpper(), out OrderStatus parsedStatus)) return BadRequest();
            var query = OrderBasicQuery().Where(o => o.RestaurantId == id && o.Status == parsedStatus);
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/awaiting/")]
        public async Task<ActionResult<List<OrderBasicDto>>> GetAwaitingForAcceptanceOrders([FromRoute] int id) {
            var query = OrderBasicQuery().Where(o => o.RestaurantId == id && o.Status == OrderStatus.PAID);
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }

        private async Task<ActionResult<OrderDto>> GetOrderById(int orderId, int restaurantId) {
            var order = await _orderDetailsService.GetOrderById(orderId, includeDeliveryPerson: true);
            if (order == null || order.Restaurant.RestaurantId != restaurantId) return NotFound();
            return order;
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/")]
        public async Task<ActionResult<OrderDto>> GetOrder([FromRoute] int orderId, [FromRoute] int id) {
            return await GetOrderById(orderId, id);
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/accept/")]
        [HttpPost]
        public async Task<ActionResult<OrderDto>> AcceptOrder([FromRoute] int orderId, [FromRoute] int id) {
            var order = await OrderBasicQuery().Where(o => o.RestaurantId == id && o.OrderId == orderId)
                .FirstOrDefaultAsync();
            if (order == null) return NotFound();
            await _orderTaskService.TryCompleteTask(order, OrderTaskType.ACCEPT, MsiUser);
            await _dbContext.SaveChangesAsync();
            return await GetOrderById(orderId, id);
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/prepare/")]
        [HttpPost]
        public async Task<ActionResult<OrderDto>> PrepareOrder([FromRoute] int orderId, [FromRoute] int id) {
            var order = await OrderBasicQuery().Where(o => o.RestaurantId == id && o.OrderId == orderId)
                .FirstOrDefaultAsync();
            if (order == null) return NotFound();
            await _orderTaskService.TryCompleteTask(order, OrderTaskType.PREPARE, MsiUser);
            await _dbContext.SaveChangesAsync();
            return await GetOrderById(orderId, id);
        }

        private async Task<List<DelivererDto>> GetFreeDeliverersList(int id) {
            var deliverers = _dbContext.RestaurantUsers.Where(ru => ru.RestaurantId == id && ru.CanDeliverOrders);
            Dictionary<RestaurantUser, int> activeTasks = new();
            foreach (var deliverer in deliverers) {
                var count = await _dbContext.OrderTasks.CountAsync(ot =>
                    ot.AssigneeUserId == deliverer.UserId
                    && ot.AssigneeType == AssigneeType.DELIVERY
                    && ot.DateCompleted != null);
                activeTasks.Add(deliverer, count);
            }

            var freeDeliverers = activeTasks.ToList();
            freeDeliverers.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            return freeDeliverers.Select(d => d.Key.AsDelivererDto(d.Value)).ToList();
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/freedeliverers/")]
        public async Task<ActionResult<List<DelivererDto>>> GetFreeDeliverers([FromRoute] int id) {
            return await GetFreeDeliverersList(id);
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/delivery/")]
        [HttpPost]
        public async Task<ActionResult<OrderDto>> AssignOrder([FromRoute] int orderId, [FromRoute] int id,
            [FromQuery] string assign, [FromBody] int deliverId) {
            var order = await _dbContext.Orders.Where(o => o.RestaurantId == id && o.OrderId == orderId)
                .FirstOrDefaultAsync();
            if (order == null) return NotFound();
            OrderTask orderTask =
                new OrderTask {Task = OrderTaskType.DELIVER, OrderId = orderId, AssigneeRestaurantId = id};
            switch (assign) {
                case "auto":
                    orderTask.AssigneeType = AssigneeType.DELIVERY;
                    var freeDeliverers = await GetFreeDeliverersList(id);
                    if (freeDeliverers.Count == 0) {
                        return Problem("No free deliverers.");
                    } else {
                        orderTask.AssigneeUserId = freeDeliverers[0].User.UserId;
                    }

                    break;
                case "specified":
                    orderTask.AssigneeType = AssigneeType.DELIVERY;
                    orderTask.AssigneeUserId = deliverId;
                    break;
                case "unassigned":
                    orderTask.AssigneeType = AssigneeType.RESTAURANT;
                    break;
            }

            await _dbContext.OrderTasks.AddAsync(orderTask);
            order.Updated = Utils.Now();
            await _dbContext.SaveChangesAsync();

            return await GetOrderById(orderId, id);
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/")]
        [HttpDelete]
        public async Task<ActionResult<OrderDto>> RejectOrder([FromRoute] int orderId, [FromRoute] int id) {
            var order = await _dbContext.Orders.Where(o => o.RestaurantId == id && o.OrderId == orderId)
                .FirstOrDefaultAsync();
            if (order == null) return NotFound();
            await _orderTaskService.TryCompleteTask(order, OrderTaskType.REJECT, MsiUser);
            await _dbContext.SaveChangesAsync();
            return await GetOrderById(orderId, id);
        }
    }
}
