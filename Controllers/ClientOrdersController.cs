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
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [Authorize]
    [ApiController]
    [Route(template: "api/orders/")]
    public class ClientOrdersController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        private readonly PaymentService _paymentService;
        private readonly OrderTaskService _orderTaskService;
        
        public ClientOrdersController(MsiDbContext dbContext, PaymentService paymentService, OrderTaskService orderTaskService) {
            _dbContext = dbContext;
            _paymentService = paymentService;
            _orderTaskService = orderTaskService;
        }
        
        [Route(template: "")]
        public async Task<ActionResult<List<OrderBasicDto>>> AllOrders() {
            var query = _dbContext.Orders.Where(o => o.CustomerId==MsiUserId); 
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }
        
        [Route(template: "")]
        public async Task<ActionResult<List<OrderBasicDto>>> FilteredOrders([FromQuery] string status) {
            OrderStatus parsedStatus;
            Enum.TryParse(status.ToUpper(), out parsedStatus);
            var query = _dbContext.Orders.Where(o => o.CustomerId==MsiUserId && 
                                                     o.Status.Equals(parsedStatus)); 
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }
        [Route(template: "active/")]
        public async Task<ActionResult<List<OrderBasicDto>>> ActiveOrders() {
            OrderStatus parsedStatus;
            var query = _dbContext.Orders.Where(o => o.CustomerId==MsiUserId && 
                                                     !(o.Status.Equals(OrderStatus.REJECTED) ||
                                                      o.Status.Equals(OrderStatus.CANCELLED) ||
                                                      o.Status.Equals(OrderStatus.COMPLETED))); 
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }
        
        [Route(template: "{id}/")]
        public async Task<ActionResult<OrderDto>> GetOrder([FromRoute] int id) {
            Order order = await _dbContext.Orders.FindAsync(id);
            var query =  _dbContext.OrderItems.Where(oi => oi.OrderId==id && oi.Order.CustomerId==MsiUserId);
            List<OrderItemCustomization> options = new List<OrderItemCustomization>();
            var oiList = await query.ToListAsync();
            foreach (var item in oiList) {
                var option = _dbContext.OrderItemCustomizations.Where(oic => oic.OrderItemId == item.OrderItemId);
                var oicList = await option.ToListAsync();
                foreach (var optionItem in oicList) {
                    options.Add(optionItem);
                }
            }
            return order.AsDto(options, oiList);
        }
        
        [Route(template: "{id}/")]
        [HttpDelete]
        public async Task<ActionResult<OrderBasicDto>> CancelOrder([FromRoute] int id) {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null) return NotFound();
            if (order.Status == OrderStatus.PAID || order.Status == OrderStatus.CREATED) {
                await _orderTaskService.TryCompleteTask(order, OrderTaskType.CANCEL, MsiUser);
                await  _paymentService.ReturnOrderPayment(order);
                order.Updated = new ZonedDateTime();
            }

            await _dbContext.SaveChangesAsync();
            return order.AsBasicDto();
        }
        
        [Route(template: "")]
        [HttpPost]
        public async Task<ActionResult<OrderBasicDto>> CreateOrder([FromBody] OrderCreatingDto ocDto) {
            Order order = new Order(){
                DeliveryNotes = ocDto.DeliveryNotes,
                RestaurantId = ocDto.RestaurantId,
                AddressId = ocDto.AddressId,
                CustomerId = ocDto.CustomerId,
                Created = new ZonedDateTime()};
            await _dbContext.Orders.AddAsync(order);
            decimal totalPrice = 0;
            foreach (var oi in ocDto.Items) {
                OrderItem orderItem = new OrderItem() {OrderId = order.OrderId, MenuItemId = oi.MenuItemId};
                await _dbContext.OrderItems.AddAsync(orderItem);
                totalPrice += oi.TotalPrice;
            }
            foreach (var opi in ocDto.OptionItems) {
                OrderItemCustomization orderOption = new OrderItemCustomization() {OrderItemId = opi.OrderItemId,
                    MenuItemOptionItemId = opi.OrderItemCustomizationId};
                await _dbContext.OrderItemCustomizations.AddAsync(orderOption);
                var optionItem = await _dbContext.MenuItemOptionItems.FindAsync(orderOption.MenuItemOptionItemId);
                totalPrice += optionItem.Price;
            }
            order.TotalPrice = totalPrice;
            await _orderTaskService.TryCompleteTask(order, OrderTaskType.CREATE, MsiUser);
            await _paymentService.CreatePaymentForOrder(order);
            await _dbContext.SaveChangesAsync();
            return order.AsBasicDto();
        }

        [Route(template: "{id}/")]
        [HttpPut]
        public async Task<ActionResult<OrderDto>> PayForOrder([FromRoute] int id) {
            var order = await _dbContext.Orders.FindAsync(id);
            var query =  _dbContext.OrderItems.Where(oi => oi.OrderId==id && oi.Order.CustomerId==MsiUserId);
            List<OrderItemCustomization> options = new List<OrderItemCustomization>();
            var oiList = await query.ToListAsync();
            foreach (var item in oiList) {
                var option = _dbContext.OrderItemCustomizations.Where(oic => oic.OrderItemId == item.OrderItemId);
                var oicList = await option.ToListAsync();
                foreach (var optionItem in oicList) {
                    options.Add(optionItem);
                }
            }
            var payment = await _dbContext.Payments.Where(p => p.OrderId == id).FirstAsync();
            await _paymentService.MakePayment(payment);
            await _orderTaskService.TryMarkAsPaid(id);
            return order.AsDto(options, oiList);
        }
    }
}
