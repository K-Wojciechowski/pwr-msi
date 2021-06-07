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
    public class RestaurantOrderController : OrdersControllerBase {
        private readonly OrderTaskService _orderTaskService;
        protected override bool IncludeDeliveryPerson => true;

        public RestaurantOrderController(MsiDbContext dbContext, OrderDetailsService orderDetailsService,
            OrderTaskService orderTaskService) {
            _dbContext = dbContext;
            _orderDetailsService = orderDetailsService;
            _orderTaskService = orderTaskService;
        }


        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/")]
        public async Task<ActionResult<Page<OrderBasicDto>>> GetAllOrders([FromRoute] int id, [FromQuery] int page) {
            var query = _dbContext.Orders.Where(o => o.RestaurantId == id);
            return await PaginateBasicOrders(query, page);
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/awaiting/")]
        public async Task<ActionResult<List<OrderBasicDto>>> GetAwaitingForAcceptanceOrders([FromRoute] int id) {
            var query = _dbContext.Orders.Where(o => o.RestaurantId == id && o.Status == OrderStatus.PAID);
            return await GetBasicOrders(query);
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/preparing/")]
        public async Task<ActionResult<List<OrderBasicDto>>> GetOrdersInPreparing([FromRoute] int id) {
            var query = _dbContext.Orders.Where(o => o.RestaurantId == id && o.Status == OrderStatus.ACCEPTED);
            return await GetBasicOrders(query);
        }


        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/delivery/")]
        public async Task<ActionResult<List<OrderBasicDto>>> GetOrdersInDelivery([FromRoute] int id) {
            var query = _dbContext.Orders.Where(o => o.RestaurantId == id && o.Status == OrderStatus.PREPARED);
            return await GetBasicOrders(query);
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
            var order = await _dbContext.Orders.Where(o => o.RestaurantId == id && o.OrderId == orderId)
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
            var order = await _dbContext.Orders.Where(o => o.RestaurantId == id && o.OrderId == orderId)
                .FirstOrDefaultAsync();
            if (order == null) return NotFound();
            await _orderTaskService.TryCompleteTask(order, OrderTaskType.PREPARE, MsiUser);
            await _dbContext.SaveChangesAsync();
            return await GetOrderById(orderId, id);
        }

        private async Task<List<DelivererDto>> GetFreeDeliverersList(int id) {
            var random = new Random();
            var deliverers = await _dbContext.RestaurantUsers.Where(ru => ru.RestaurantId == id && ru.CanDeliverOrders)
                .Select(ru => ru.UserId).ToListAsync();
            var delivererUsers =
                (await _dbContext.Users.Where(u => deliverers.Contains(u.UserId)).ToListAsync()).ToDictionary(u =>
                    u.UserId);
            var deliverersWithTasks = await _dbContext.OrderTasks.Where(ot =>
                    ot.AssigneeUserId != null && deliverers.Contains(ot.AssigneeUserId!.Value) &&
                    ot.AssigneeType == AssigneeType.DELIVERY && ot.DateCompleted == null)
                .GroupBy(ot => ot.AssigneeUserId!.Value)
                .OrderBy(g => g.Count())
                .Select(g => new DelivererDto(delivererUsers[g.Key], g.Count()))
                .ToListAsync();
            var deliverersWithTasksIds = deliverersWithTasks.Select(d => d.User.UserId);
            var remainingDeliverersIds = deliverers.Except(deliverersWithTasksIds);
            var deliverersWithoutTasks = remainingDeliverersIds
                .Select(i => new DelivererDto(delivererUsers[i], activeTasks: 0))
                .OrderBy(_ => random.Next()).ToList();
            var allDeliverers = new List<DelivererDto>();
            allDeliverers.AddRange(deliverersWithoutTasks);
            allDeliverers.AddRange(deliverersWithTasks);
            return allDeliverers;
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/deliverers/")]
        public async Task<ActionResult<List<DelivererDto>>> GetFreeDeliverers([FromRoute] int id) {
            return await GetFreeDeliverersList(id);
        }

        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/delivery/")]
        [HttpPost]
        public async Task<ActionResult<OrderDto>> AssignOrder([FromRoute] int orderId, [FromRoute] int id,
            [FromQuery] string assign, [FromQuery(Name = "id")] int deliverId) {
            var order = await _dbContext.Orders.Where(o => o.RestaurantId == id && o.OrderId == orderId)
                .FirstOrDefaultAsync();
            if (order == null) return NotFound();
            bool status;
            switch (assign) {
                case "auto":
                    var freeDeliverers = await GetFreeDeliverersList(id);
                    if (freeDeliverers.Count == 0) {
                        return Problem("No free deliverers.");
                    } else {
                        var assignee = freeDeliverers.First().User.UserId;
                        status = await _orderTaskService.TryRegisterOrAssignTask(order, OrderTaskType.DELIVER,
                            AssigneeType.DELIVERY, assigneeUserId: assignee);
                    }

                    break;
                case "specified":
                    status = await _orderTaskService.TryRegisterOrAssignTask(order, OrderTaskType.DELIVER,
                        AssigneeType.DELIVERY, assigneeUserId: deliverId);
                    break;
                case "unassigned":
                    status = await _orderTaskService.TryRegisterOrAssignTask(order, OrderTaskType.DELIVER,
                        AssigneeType.RESTAURANT, assigneeRestaurantId: order.RestaurantId);
                    break;
                default:
                    status = false;
                    break;
            }

            if (!status) return Problem("Unable to assign.");
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
