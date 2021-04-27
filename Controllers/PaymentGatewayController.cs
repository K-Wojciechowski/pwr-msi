using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models.Dto.PaymentGateway;

namespace pwr_msi.Controllers {
    [ApiController]
    [Route(template: "api/paymentgateway/")]
    public class PaymentGatewayController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;

        public PaymentGatewayController(MsiDbContext dbContext) {
            _dbContext = dbContext;
        }

        [Route("callback/")]
        [HttpPost]
        public async Task<IActionResult> paymentGatewayCallback([FromBody] PaymentCallbackDto paymentCallbackDto) {
            var payment = await _dbContext.Payments.Where(p => p.ExternalPaymentId == paymentCallbackDto.Id)
                .FirstOrDefaultAsync();
            if (payment == null) {
                return NotFound();
            }

            payment.Status = paymentCallbackDto.Status;
            payment.ErrorMessage = paymentCallbackDto.Error;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
