using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Enum;

namespace pwr_msi.Controllers {
    [Authorize]
    [AdminAuthorize]
    [ApiController]
    [Route(template: "api/restaurant/")]
    public class RestaurantOrderController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;

        public RestaurantOrderController(MsiDbContext dbContext) {
            _dbContext = dbContext;

        }
        
        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/")]
        public async Task<ActionResult<List<OrderBasicDto>>> GetAllOrders([FromRoute] int id) {
            var query = _dbContext.Orders.Where(o => o.RestaurantId == id);
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }
        
        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/")]
        public async Task<ActionResult<List<OrderBasicDto>>> GetOrders([FromRoute] int id, [FromQuery] string status) {
            OrderStatus parsedStatus;
            var query = _dbContext.Orders.Where(o => o.RestaurantId == id && o.Status.Equals(Enum.TryParse(status.ToUpper(), out parsedStatus)));
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }
        
        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrder([FromRoute] int orderId, [FromRoute] int id) {
            var order = await _dbContext.Orders.Where(o => o.RestaurantId == id && o.OrderId == orderId).FirstOrDefaultAsync();
            var query = _dbContext.OrderItems.Where(oi => oi.OrderId == orderId);
            ICollection<OrderItem> items = await query.ToListAsync(); 
            return order == null ? NotFound() : order.AsDetailedDto(items);
        }
        
        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/")]
        [HttpPut]
        public async Task<ActionResult<OrderBasicDto>> AcceptOrder([FromRoute] int orderId, [FromRoute] int id, [FromQuery] string status) {
            var order = await _dbContext.Orders.Where(o => o.RestaurantId == id && o.OrderId == orderId).FirstOrDefaultAsync();
            if (order == null) return NotFound();
            switch (status) {
                case("accepted"):
                    order.Status = OrderStatus.ACCEPTED;
                    break;
                case("prepared") :
                    order.Status = OrderStatus.PREPARED;
                    break;
            }
            await _dbContext.SaveChangesAsync();
            return order.AsBasicDto();
        }
        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/delivery/")]
        public async Task<ActionResult<List<DelivererDto>>> GetDeliverers([FromRoute] int orderId, [FromRoute] int id) {
            var order = await _dbContext.Orders.Where(o => o.RestaurantId == id && o.OrderId == orderId).FirstOrDefaultAsync();
            if (order == null) return NotFound();
            var deliverers = _dbContext.RestaurantUsers.Where(ru => ru.RestaurantId == id && ru.CanDeliverOrders);
            Dictionary<RestaurantUser, int> activeTasks = new Dictionary<RestaurantUser, int>();
            ZonedDateTime now = new ZonedDateTime();
            foreach (var deliverer in deliverers) {
                var count = await _dbContext.OrderTasks.CountAsync(ot => ot.AssigneeUserId == deliverer.UserId 
                                                                         && ot.AssigneeType.Equals(AssigneeType.DELIVERY)
                                                                         && (ZonedDateTime.Comparer.Local.Compare((ZonedDateTime)ot.DateCompleted, now) >= 0));
                activeTasks.Add(deliverer, count);
            }
            var unbusyDeliverers = activeTasks.ToList();
            unbusyDeliverers.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));
            return unbusyDeliverers.Select(d => d.Key.AsDelivererDto(d.Value)).ToList();;
        }
        
        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/delivery/")]
        [HttpPut]
        public async Task<ActionResult<OrderDetailsDto>> AssignOrder([FromRoute] int orderId, [FromRoute] int id, [FromQuery] string assign, [FromBody] int deliverId) {
            var order = await _dbContext.Orders.Where(o => o.RestaurantId == id && o.OrderId == orderId).FirstOrDefaultAsync();
            if (order == null) return NotFound();
            OrderTask orderTask =
                new OrderTask {Task = OrderTaskType.DELIVER, OrderId = orderId, AssigneeRestaurantId = id};
            switch (assign) {
                case("auto"):
                    orderTask.AssigneeType = AssigneeType.DELIVERY;
                    var deliverers = _dbContext.RestaurantUsers.Where(ru => ru.RestaurantId == id && ru.CanDeliverOrders);
                    Dictionary<RestaurantUser, int> activeTasks = new Dictionary<RestaurantUser, int>();
                    ZonedDateTime now = new ZonedDateTime();
                    foreach (var deliverer in deliverers) {
                        var count = await _dbContext.OrderTasks.CountAsync(ot => ot.AssigneeUserId == deliverer.UserId 
                                                                           && ot.AssigneeType.Equals(AssigneeType.DELIVERY)
                                                                           && (ZonedDateTime.Comparer.Local.Compare((ZonedDateTime)ot.DateCompleted, now) >= 0));
                        activeTasks.Add(deliverer, count);
                    }
                    var unbusyDeliverers = activeTasks.ToList();
                    unbusyDeliverers.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));
                    orderTask.AssigneeUserId = unbusyDeliverers[0].Key.UserId;
                    break;
                case("specified") :
                    orderTask.AssigneeType = AssigneeType.DELIVERY;
                    orderTask.AssigneeUserId = deliverId;
                    break;
                case("unassigned") :
                    orderTask.AssigneeType = AssigneeType.RESTAURANT;
                    break;
            }
            await _dbContext.OrderTasks.AddAsync(orderTask);
            var query = _dbContext.OrderItems.Where(oi => oi.OrderId == orderId);
            ICollection<OrderItem> items = await query.ToListAsync(); 
            await _dbContext.SaveChangesAsync();
            return order.AsDetailedDto(items);
        }
        
        [AcceptOrdersRestaurantAuthorize("id")]
        [Route(template: "{id}/orders/{orderId}/")]
        [HttpDelete]
        public async Task<ActionResult<OrderBasicDto>> RejectOrder([FromRoute] int orderId, [FromRoute] int id) {
            var order = await _dbContext.Orders.Where(o => o.RestaurantId == id && o.OrderId == orderId).FirstOrDefaultAsync();
            if (order == null) return NotFound();
            order.Status = OrderStatus.REJECTED;
            await _dbContext.SaveChangesAsync();
            return order.AsBasicDto();
        }
    }
}
