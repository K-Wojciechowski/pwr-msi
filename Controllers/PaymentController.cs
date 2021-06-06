#nullable enable
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
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [ApiController]
    [Authorize]
    [Route(template: "api/payments/")]
    public class PaymentController : MsiControllerBase {
        private MsiDbContext _dbContext;
        private PaymentService _paymentService;
        private OrderDetailsService _orderDetailsService;

        public PaymentController(MsiDbContext dbContext, PaymentService paymentService, OrderDetailsService orderDetailsService) {
            _dbContext = dbContext;
            _paymentService = paymentService;
            _orderDetailsService = orderDetailsService;
        }

        private async Task<OrderBasicDto?> GetOrderBasicById(int? id) {
            if (id == null) return null;
            var order = await _orderDetailsService.GetOrderById(id.Value, includeDeliveryPerson: false);
            return order?.AsBasicDto();
        }

        private IQueryable<Payment> GetPaymentBaseQueryable() {
            return _dbContext.Payments.Include(p => p.Order).Include("Order.Customer").Include("Order.Restaurant");
        }

        [Route("")]
        public async Task<Page<PaymentDto>> Payments([FromQuery] int page = 1) {
            return await Utils.PaginateAsync(
                queryable:
                GetPaymentBaseQueryable()
                    .Where(p => p.UserId == MsiUserId)
                    .OrderByDescending(p => p.Created),
                pageRaw: page,
                converter: async p => p.AsDto(await GetOrderBasicById(p.OrderId))
            );
        }

        [Route("pending/")]
        public async Task<IEnumerable<PaymentDto>> PendingPayments() {
            var paymentsQuery = GetPaymentBaseQueryable().Where(p =>
                p.UserId == MsiUserId &&
                (p.Status == PaymentStatus.CREATED || p.Status == PaymentStatus.REQUESTED));
            var payments = await paymentsQuery.ToListAsync();
            if (payments == null) {
                return new List<PaymentDto>();
            }

            var paymentTasks = payments!.Select(async p => p.AsDto(await GetOrderBasicById(p.OrderId)));
            return await Task.WhenAll(paymentTasks);
        }

        [Route("{id}/")]
        [HttpGet]
        public async Task<PaymentDto> GetPayment([FromRoute] int id) {
            var payment = await GetPaymentBaseQueryable().Where(p => p.UserId == MsiUserId && p.PaymentId == id
            ).FirstOrDefaultAsync();
            payment = await _paymentService.RefreshStatusFromApi(payment);
            return payment.AsDto(await GetOrderBasicById(payment.OrderId));
        }

        [Route("{id}/")]
        [HttpPost]
        public async Task<ActionResult<PaymentAttemptDto>> MakePayment([FromRoute] int id) {
            var payment = await _dbContext.Payments.Where(p => p.UserId == MsiUserId && p.PaymentId == id
            ).FirstOrDefaultAsync();
            if (payment == null) return NotFound();
            if (!payment.CanPay) return BadRequest();
            return await _paymentService.MakePayment(payment);
        }

        [Route("{id}/")]
        [HttpDelete]
        public async Task<IActionResult> CancelPayment([FromRoute] int id) {
            var payment = await _dbContext.Payments.Where(p => p.UserId == MsiUserId && p.PaymentId == id
            ).FirstOrDefaultAsync();
            if (payment == null) return NotFound();
            if (!payment.CanPay) return BadRequest();
            payment.Status = PaymentStatus.CANCELLED;
            payment.ErrorMessage = "USER_CANCELLED";
            payment.Updated = Utils.Now();
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [Route("balance/")]
        public ResultDto<decimal> GetBalance() {
            return new(MsiUser.Balance);
        }

        [Route("balance/repay/")]
        [HttpPost]
        public async Task<ActionResult<PaymentAttemptDto>> RepayBalance() {
            Debug.Assert(MsiUserId != null, nameof(MsiUserId) + " != null");
            var repayment = await _paymentService.RepayBalance(MsiUserId.Value);
            return repayment == null ? BadRequest() : repayment;
        }
    }
}
