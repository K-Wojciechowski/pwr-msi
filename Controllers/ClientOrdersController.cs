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
    [AdminAuthorize]
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
            var query = _dbContext.Orders.Where(o => o.CustomerId==MsiUserId && 
                                                     o.Status.Equals(Enum.TryParse(status.ToUpper(), out parsedStatus))); 
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }
        
        [Route(template: "{id}/")]
        public async Task<ActionResult<List<OrderItemDto>>> OrderItems([FromRoute] int id) {
            var query =  _dbContext.OrderItems.Where(oi => oi.OrderId==id && oi.Order.CustomerId==MsiUserId);
            var oiList = await query.ToListAsync();
            return oiList.Select(o => o.AsDto()).ToList();
        }
        
        [Route(template: "{id}/")]
        [HttpDelete]
        public async Task<ActionResult<OrderBasicDto>> CancelOrder([FromRoute] int id) {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null) return NotFound();
            if (order.Status == OrderStatus.PAID || order.Status == OrderStatus.CREATED ||
                order.Status == OrderStatus.DECIDED || order.Status == OrderStatus.ACCEPTED) {
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
                OrderItem orderItem = new OrderItem() {OrderId = order.OrderId};
                await _dbContext.OrderItems.AddAsync(orderItem);
                totalPrice += oi.TotalPrice;
            }
            foreach (var opi in ocDto.OptionItems) {
                totalPrice += opi.Price;
            }
            order.TotalPrice = totalPrice;
            await _orderTaskService.TryCompleteTask(order, OrderTaskType.CREATE, MsiUser);
            await _paymentService.CreatePaymentForOrder(order);
            await _dbContext.SaveChangesAsync();
            return order.AsBasicDto();
        }

        [Route(template: "{id}/")]
        [HttpPut]
        public async Task<ActionResult<OrderBasicDto>> PayForOrder([FromRoute] int id) {
            var order = await _dbContext.Orders.FindAsync(id);
            var payment = await _dbContext.Payments.Where(p => p.OrderId == id).FirstAsync();
            await _paymentService.MakePayment(payment);
            await _orderTaskService.TryMarkAsPaid(id);
            return order.AsBasicDto();
        }
    }
}
