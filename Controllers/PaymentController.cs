using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [ApiController]
    [Route(template: "api/payments/")]
    public class PaymentController : MsiControllerBase {
        private MsiDbContext _dbContext;
        private PaymentService _paymentService;

        public PaymentController(MsiDbContext dbContext, PaymentService paymentService) {
            _dbContext = dbContext;
            _paymentService = paymentService;
        }

        [Route("/")]
        public async Task<Page<PaymentDto>> Payments([FromQuery] int page = 1) {
            return await Utils.Paginate(
                queryable: _dbContext.Payments.Where(p => p.Order.CustomerId == MsiUserId),
                pageRaw: page,
                converter: p => p.AsDto()
            );
        }

        [Route("pending/")]
        public async Task<IEnumerable<PaymentDto>> PendingPayments() {
            var paymentsQuery = _dbContext.Payments.Where(p =>
                p.Order.CustomerId == MsiUserId &&
                (p.Status == PaymentStatus.CREATED || p.Status == PaymentStatus.REQUESTED));
            var payments = await paymentsQuery.ToListAsync();
            return payments.Select(p => p.AsDto());
        }

        [Route("{id}/")]
        [HttpGet]
        public async Task<PaymentDto> GetPayment([FromRoute] int id) {
        var payment = await _dbContext.Payments.Where(p => p.Order.CustomerId == MsiUserId && p.PaymentId == id
                    ).FirstOrDefaultAsync();
            payment = await _paymentService.RefreshStatusFromApi(payment);
            return payment.AsDto();
        }

        [Route("{id}/")]
        [HttpPost]
        public async Task<ActionResult<PaymentAttemptDto>> MakePayment([FromRoute] int id) {
            var payment = await _dbContext.Payments.Where(p => p.Order.CustomerId == MsiUserId && p.PaymentId == id
            ).FirstOrDefaultAsync();
            if (payment == null) return NotFound();
            if (!payment.CanPay) return BadRequest();
            return await _paymentService.MakePayment(payment);
        }
    }
}
