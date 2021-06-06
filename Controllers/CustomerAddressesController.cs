#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;
using pwr_msi.Models.Dto;

namespace pwr_msi.Controllers {
    [Authorize]
    [ApiController]
    [Route(template: "api/addresses/")]
    public class ClientAddressController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        
        public ClientAddressController(MsiDbContext dbContext) {
            _dbContext = dbContext;
        }
        
        [Route(template: "")]
        public async Task<ActionResult<List<Address>>> GetAddresses() {
            var user = await _dbContext.Users.Include(u => u.Addresses).Where(u => u.UserId == MsiUserId)
                .FirstOrDefaultAsync();
            return user.Addresses.ToList();
        }

        [Route(template: "default/")]
        public async Task<ActionResult<Address>> GetDefaultAddress() {
            if (MsiUser.BillingAddressId == null) return NotFound();
            return await _dbContext.Addresses.FindAsync(MsiUser.BillingAddressId);
        }


        [Route(template: "default/")]
        [HttpPost]
        public async Task<ActionResult<Address>> SetDefaultAddress([FromBody] InputDto<int> addressIdInput) {
            var user = await _dbContext.Users.FindAsync(MsiUserId);
            user.BillingAddressId = addressIdInput.Input;
            await _dbContext.SaveChangesAsync();
            return await _dbContext.Addresses.FindAsync(user.BillingAddressId);
        }

        [Route(template: "")]
        [HttpPost]
        public async Task<ActionResult<Address>> AddAddress([FromBody] Address address) {
            address.Users.Add(MsiUser);
            await _dbContext.Addresses.AddAsync(address);
            await _dbContext.SaveChangesAsync();
            return address;
        }

        [Route(template: "{id}/")]
        [HttpPut]
        public async Task<ActionResult<Address>> ModifyAddress([FromBody] Address inputAddress, [FromRoute] int id) {
            var address = await  _dbContext.Addresses.FindAsync(id);
            address.Update(inputAddress);
            await _dbContext.SaveChangesAsync();
            return address;
        }
        
        [Route(template: "{id}/")]
        [HttpDelete]
        public async Task<ActionResult> DeleteAddress([FromRoute] int id) {
            var address = await  _dbContext.Addresses.FindAsync(id);
            _dbContext.Remove(address);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        
    }
}
