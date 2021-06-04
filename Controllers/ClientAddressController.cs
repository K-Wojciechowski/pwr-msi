using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;
using pwr_msi.Models.Dto;

namespace pwr_msi.Controllers {
    [Authorize]
    [ApiController]
    [Route(template: "api/address/")]
    public class ClientAddressController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        
        public ClientAddressController(MsiDbContext dbContext) {
            _dbContext = dbContext;
        }
        
        [Route(template: "")]
        public async Task<ActionResult<List<AddressDto>>> GetAddresses() {
            List<Address> addresses = MsiUser.Addresses.ToList();
            return  addresses.Select(a => a.AsDto()).ToList();
        }
        
        [Route(template: "")]
        [HttpPost]
        public async Task<ActionResult<AddressDto>> AddAddress([FromBody] AddressDto ad) {
            var address =  await _dbContext.Addresses.FirstAsync(a => a.Addressee == ad.Addressee
                                                                      && a.City == ad.City
                                                                      && a.Country == ad.Country
                                                                      && a.FirstLine == ad.FirstLine
                                                                      && a.SecondLine == ad.SecondLine
                                                                      && a.PostCode == ad.PostCode);
            if (address==null) {
                address = ad.AsNewAddress();
                await _dbContext.Addresses.AddAsync(address);
            }
            address.Users.Add(MsiUser);
            await _dbContext.SaveChangesAsync();
            return address.AsDto();
        }
        [Route(template: "{id}/")]
        [HttpPut]
        public async Task<ActionResult<AddressDto>> ModifyAddress([FromBody] AddressDto ad, [FromRoute] int id) {
            var address =  await _dbContext.Addresses.FirstAsync(a => a.Addressee == ad.Addressee
                                                                      && a.City == ad.City
                                                                      && a.Country == ad.Country
                                                                      && a.FirstLine == ad.FirstLine
                                                                      && a.SecondLine == ad.SecondLine
                                                                      && a.PostCode == ad.PostCode);
            if (address==null) {
                address = ad.AsNewAddress();
                await _dbContext.Addresses.AddAsync(address);
            }

            var oldAddress = await  _dbContext.Addresses.FindAsync(id);
            oldAddress.Users.Remove(MsiUser);
            if (oldAddress.Users.IsNullOrEmpty()) {
                _dbContext.Addresses.Remove(oldAddress);
            }
            address.Users.Add(MsiUser);
            await _dbContext.SaveChangesAsync();
            return address.AsDto();
        }
        
        [Route(template: "{id}/")]
        [HttpDelete]
        public async Task<ActionResult<AddressDto>> DeleteAddress([FromRoute] int id) {
            var oldAddress = await  _dbContext.Addresses.FindAsync(id);
            oldAddress.Users.Remove(MsiUser);
            if (oldAddress.Users.IsNullOrEmpty()) {
                _dbContext.Addresses.Remove(oldAddress);
            }
            await _dbContext.SaveChangesAsync();
            return oldAddress.AsDto();
        }
        
    }
}
