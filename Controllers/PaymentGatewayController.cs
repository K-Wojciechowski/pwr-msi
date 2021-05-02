#nullable enable
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models.Dto.PaymentGateway;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [ApiController]
    [Route(template: "api/paymentgateway/")]
    public class PaymentGatewayController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        private readonly PaymentService _paymentService;

        public PaymentGatewayController(MsiDbContext dbContext, PaymentService paymentService) {
            _dbContext = dbContext;
            _paymentService = paymentService;
        }

        [Route("callback/")]
        [HttpPost]
        public async Task<IActionResult> paymentGatewayCallback([FromBody] PaymentCallbackDto paymentCallbackDto) {
            var payment = await _dbContext.Payments.Where(p => p.ExternalPaymentId == paymentCallbackDto.Id)
                .FirstOrDefaultAsync();
            if (payment == null) {
                return NotFound();
            }

            await _paymentService.HandlePaymentCallback(payment.PaymentId, paymentCallbackDto.Status, paymentCallbackDto.Error);
            return Ok();
        }
    }
}
