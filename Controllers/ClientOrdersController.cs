using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly OrderDetailsService _orderDetailsService;
        private readonly OrderTaskService _orderTaskService;

        public ClientOrdersController(MsiDbContext dbContext, PaymentService paymentService,
            OrderDetailsService orderDetailsService,
            OrderTaskService orderTaskService) {
            _dbContext = dbContext;
            _paymentService = paymentService;
            _orderDetailsService = orderDetailsService;
            _orderTaskService = orderTaskService;
        }

        private IQueryable<Order> OrderBasicQuery() {
            return _dbContext.Orders.Include(o => o.Customer).Include(o => o.Restaurant).Include(o => o.Address);
        }

        [Route(template: "")]
        public async Task<ActionResult<List<OrderBasicDto>>> AllOrders() {
            var query = OrderBasicQuery().Where(o => o.CustomerId == MsiUserId);
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }

        [Route(template: "")]
        public async Task<ActionResult<List<OrderBasicDto>>> FilteredOrders([FromQuery] string status) {
            if (!Enum.TryParse(status.ToUpper(), out OrderStatus parsedStatus)) return BadRequest();
            var query = OrderBasicQuery().Where(
                o => o.CustomerId == MsiUserId &&
                     o.Status == parsedStatus);
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }

        [Route(template: "active/")]
        public async Task<ActionResult<List<OrderBasicDto>>> ActiveOrders() {
            var query = OrderBasicQuery().Where(o => o.CustomerId == MsiUserId)
                .Where(o =>
                    o.Status != OrderStatus.REJECTED &&
                    o.Status != OrderStatus.CANCELLED &&
                    o.Status != OrderStatus.COMPLETED);
            var oList = await query.ToListAsync();
            return oList.Select(o => o.AsBasicDto()).ToList();
        }

        [Route(template: "{id}/")]
        public async Task<ActionResult<OrderDto>> GetOrder([FromRoute] int id) {
            OrderDto orderDto = await _orderDetailsService.GetOrderById(id);
            if (orderDto == null || orderDto.Customer.UserId != MsiUserId) return NotFound();
            return orderDto;
        }

        [Route(template: "{id}/")]
        [HttpDelete]
        public async Task<ActionResult<OrderBasicDto>> CancelOrder([FromRoute] int id) {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null) return NotFound();
            if (order.Status == OrderStatus.PAID || order.Status == OrderStatus.CREATED) {
                await _orderTaskService.TryCompleteTask(order, OrderTaskType.CANCEL, MsiUser);
                await _paymentService.ReturnOrderPayment(order);
                order.Updated = new ZonedDateTime();
                await _dbContext.SaveChangesAsync();
                return order.AsBasicDto();
            }

            return BadRequest();
        }

        [Route(template: "")]
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderDto orderDto) {
            Debug.Assert(MsiUserId != null, nameof(MsiUserId) + " != null");
            var order = await orderDto.AsNewOrder(MsiUserId.Value, _dbContext);

            await _dbContext.Orders.AddAsync(order);
            await _orderTaskService.TryCompleteTask(order, OrderTaskType.CREATE, MsiUser);
            await _dbContext.SaveChangesAsync();
            return await _orderDetailsService.GetOrderById(order.OrderId);
        }

        [Route(template: "{id}/pay/")]
        [HttpPost]
        public async Task<ActionResult<PaymentGroupInfo>> PayForOrder([FromRoute] int id, [FromQuery] bool useBalance) {
            var order = await _dbContext.Orders.Include(o => o.Customer)
                .Where(o => o.OrderId == id && o.CustomerId == MsiUserId).FirstOrDefaultAsync();
            if (order == null) {
                return NotFound();
            }

            return await _paymentService.CreatePaymentForOrder(order, useBalance);
        }
    }
}
