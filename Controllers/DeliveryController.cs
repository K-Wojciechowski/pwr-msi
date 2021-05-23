using System;
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
        
        public DeliveryController(MsiDbContext dbContext, OrderTaskService orderTaskService) {
            _dbContext = dbContext;
            _orderTaskService = orderTaskService;
        }
        private async Task<List<Order>> GetCloseOrders(IQueryable<Order> query, GeoCoordinate userLoc, int range) {
            var oList = await query.ToListAsync();
            foreach (var o in oList) {
                var rAddress = new GeoCoordinate(o.Restaurant.Address.Latitude, o.Restaurant.Address.Longitude);
                if (rAddress.GetDistanceTo(userLoc) > range) {
                    oList.Remove(o);
                }
            }
            return oList;
        }

        [Route(template: "active/")]
        public async Task<ActionResult<List<OrderBasicDto>>> AllActiveOrders() {
            var query = _dbContext.Orders.Where(o => o.DeliveryPersonId==MsiUserId
                                                     && (o.Status.Equals(OrderStatus.PREPARED) 
                                                         || o.Status.Equals(OrderStatus.ACCEPTED)));
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }
        
        [Route(template: "active/")]
        public async Task<ActionResult<List<OrderBasicDto>>> FilteredActiveOrders([FromQuery] string restaurant) {
            var likeQuery = $"%{restaurant}%";
            var query = _dbContext.Orders.Where(o => o.DeliveryPersonId==MsiUserId
                                                     && (o.Status.Equals(OrderStatus.PREPARED) 
                                                         || o.Status.Equals(OrderStatus.ACCEPTED))
                                                     && EF.Functions.ILike(o.Restaurant.Name, likeQuery));
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }
        
        [Route(template: "waiting/")]
        public async Task<ActionResult<List<OrderBasicDto>>> AllWaitingOrders([FromQuery] float lat,[FromQuery] float lng, [FromQuery] int range) {
            var query = _dbContext.Orders.Where(o => o.DeliveryPerson==null
                                                     && (o.Status.Equals(OrderStatus.PREPARED) 
                                                         || o.Status.Equals(OrderStatus.ACCEPTED)));

            var userAddress = new GeoCoordinate(lat, lng);
            var oList = await GetCloseOrders(query, userAddress, range);
            return oList.Select(o => o.AsBasicDto()).ToList();
        }
        
        [Route(template: "waiting/")]
        public async Task<ActionResult<List<OrderBasicDto>>> FilteredWaitingOrders([FromQuery] string restaurant, 
            [FromQuery] float lat,[FromQuery] float lng, [FromQuery] int range) {
            var likeQuery = $"%{restaurant}%";
            var query = _dbContext.Orders.Where(o => o.DeliveryPerson==null 
                                                     && (o.Status.Equals(OrderStatus.PREPARED) 
                                                         || o.Status.Equals(OrderStatus.ACCEPTED))
                                                     && EF.Functions.ILike(o.Restaurant.Name, likeQuery));
            var userAddress = new GeoCoordinate(lat, lng);
            var oList = await GetCloseOrders(query, userAddress, range);
            return oList.Select(o => o.AsBasicDto()).ToList();
        }
        
        [Route(template: "completed/")]
        public async Task<ActionResult<List<OrderBasicDto>>> AllCompletedOrders() {
            var query = _dbContext.Orders.Where(o => o.DeliveryPerson==null 
                                                     && (o.Status.Equals(OrderStatus.COMPLETED) 
                                                         || o.Status.Equals(OrderStatus.DELIVERED)));
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }

        [Route(template: "completed/")] public async Task<ActionResult<List<OrderBasicDto>>> FilteredCompletedOrders([FromQuery] string restaurant) {
            var likeQuery = $"%{restaurant}%";
            var query = _dbContext.Orders.Where(o => o.DeliveryPerson==null 
                                                     && (o.Status.Equals(OrderStatus.COMPLETED) 
                                                         || o.Status.Equals(OrderStatus.DELIVERED))
                                                     && EF.Functions.ILike(o.Restaurant.Name, likeQuery));
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }

        [Route(template: "active/{id}/")]
        [HttpPut]
        public async Task<ActionResult<OrderBasicDto>> MarkAsDelivered([FromRoute] int id) {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null || order.DeliveryPersonId!=MsiUserId 
                              || !(order.Status.Equals(OrderStatus.PREPARED) 
                                   || order.Status.Equals(OrderStatus.ACCEPTED))) return NotFound();
            await _orderTaskService.TryCompleteTask(order, OrderTaskType.DELIVER, MsiUser);
            return order.AsBasicDto();
        }
        
        [Route(template: "waiting/{id}/")]
        [HttpPut]
        public async Task<ActionResult<OrderBasicDto>> AssignYourself([FromRoute] int id) {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null || order.DeliveryPerson!=null
                              || !(order.Status.Equals(OrderStatus.PREPARED) 
                                   || order.Status.Equals(OrderStatus.ACCEPTED))) return NotFound();
            order.DeliveryPersonId = MsiUserId;
            order.DeliveryPerson = MsiUser;
            await _dbContext.SaveChangesAsync();
            return order.AsBasicDto();
        }
    }
}
