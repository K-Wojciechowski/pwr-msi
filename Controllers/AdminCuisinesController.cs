using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pwr_msi.AuthPolicies;
using pwr_msi.Models;
using pwr_msi.Models.Dto;

namespace pwr_msi.Controllers {
    [Authorize]
    [AdminAuthorize]
    [ApiController]
    [Route(template: "api/admin/cuisines/")]
    public class AdminCuisinesController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;

        public AdminCuisinesController(MsiDbContext dbContext) {
            _dbContext = dbContext;
        }

        [Route(template: "")]
        public async Task<ActionResult<Page<Cuisine>>> List([FromQuery] int page = 1) {
            return await Utils.Paginate(
                queryable: _dbContext.Cuisines.OrderBy(c => c.Name),
                page,
                converter: r => r
            );
        }

        [Route(template: "")]
        [HttpPost]
        public async Task<ActionResult<Cuisine>> Create([FromBody] Cuisine cuisine) {
            var entityEntry = await _dbContext.Cuisines.AddAsync(cuisine);
            await _dbContext.SaveChangesAsync();
            return entityEntry.Entity;
        }

        [Route(template: "{id}/")]
        public async Task<ActionResult<Cuisine>> Get([FromRoute] int id) {
            var cuisine = await _dbContext.Cuisines.FindAsync(id);
            return cuisine == null ? NotFound() : cuisine;
        }

        [Route(template: "{id}/")]
        [HttpPut]
        public async Task<ActionResult<Cuisine>> Update([FromRoute] int id,
            [FromBody] Cuisine cuisineInput) {
            var cuisine = await _dbContext.Cuisines.FindAsync(id);
            if (cuisine == null) return NotFound();
            cuisine.Name = cuisineInput.Name;
            await _dbContext.SaveChangesAsync();
            return cuisine;
        }

        [Route(template: "{id}/")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] int id) {
            var cuisine = await _dbContext.Cuisines.FindAsync(id);
            if (cuisine == null) return NotFound();
            _dbContext.Remove(cuisine);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }


        [Route(template: "typeahead/")]
        public async Task<ActionResult<List<Cuisine>>> CuisinesTypeAhead(string query) {
            var cuisines =
                _dbContext.Cuisines.Where(c => c.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase));
            return await cuisines.ToListAsync();
        }
    }
}
