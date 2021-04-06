using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using pwr_msi.AuthPolicies;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.Admin;
using pwr_msi.Models.Dto.Auth;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [Authorize]
    [AdminAuthorize]
    [ApiController]
    [Route(template: "api/restaurant/")]
    public class ManageRestaurantController : MsiControllerBase {
        private readonly AdminCommonService _adminCommonService;
        private readonly MsiDbContext _dbContext;
        
        public ManageRestaurantController(MsiDbContext dbContext, AdminCommonService adminCommonService) {
            _dbContext = dbContext;
            _adminCommonService = adminCommonService;
        }
        
        [Route(template: "{id}/menu/")]
        public async Task<ActionResult<List<RestaurantMenuDto>>> GetMenu([FromRoute] int id) {
            var query = _dbContext.MenuCategories.Where(mc => mc.RestaurantId == id);
            var mcList = await query.ToListAsync();
            return mcList.Select(mc => mc.AsManageMenuDto()).ToList();
        }

        [Route(template: "{id}/menu/categories")]
        public async Task<ActionResult<List<RestaurantMenuCategoryDto>>> GetCategories([FromRoute] int id) {
            var query = _dbContext.MenuCategories.Where(mc => mc.RestaurantId == id);
            var mcList = await query.ToListAsync();
            return mcList.Select(mc => mc.AsManageCategoryDto()).ToList();
        }

        [Route(template: "{id}/menu/categories")]
        [HttpPost]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> CreateCategory([FromRoute] int id,
            [FromBody] RestaurantMenuCategoryDto mcDto) {
            var menuCategory = mcDto.AsNewMenuCategory(id);
            await _dbContext.MenuCategories.AddAsync(menuCategory);
            await _dbContext.SaveChangesAsync();
            return menuCategory.AsManageCategoryDto();
        }
        
        [Route(template: "{id}/menu/categories/{catId}")]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> GetCategory([FromRoute] int catId) {
            var category = await _dbContext.MenuCategories.FindAsync(catId);
            return category == null ? NotFound() : category.AsManageCategoryDto();
        }
        
        [Route(template: "{id}/menu/categories/{catId}")]
        [HttpPut]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> UpdateCategory([FromRoute] int id, [FromRoute] int catId,
            [FromBody] RestaurantMenuCategoryDto mcDto) {
            var newMenuCategory = mcDto.AsNewMenuCategory(id);
            await _dbContext.MenuCategories.AddAsync(newMenuCategory);
            var menuCategory = await _dbContext.MenuCategories.FindAsync(catId);
            if (menuCategory == null) return NotFound();
            menuCategory.UpdateWithRestaurantMenuCategoryDto(mcDto);
            await _dbContext.SaveChangesAsync();
            return newMenuCategory.AsManageCategoryDto();
        }
        [Route(template: "{id}/menu/categories/{catId}")]
        [HttpDelete]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> DeleteCategory( [FromRoute] int catId) {
            var menuCategory = await _dbContext.MenuCategories.FindAsync(catId);
            if (menuCategory == null) return NotFound();
            menuCategory.MakeMenuCategoryNonValid();
            await _dbContext.SaveChangesAsync();
            return menuCategory.AsManageCategoryDto();
        }
        

        [Route(template: "{id}/menu/items")]
        public async Task<ActionResult<List<RestaurantMenuItemDto>>> GetItems([FromRoute] int id) {
            var query = _dbContext.MenuItems.Where(mi => mi.MenuCategory.RestaurantId == id);
            var miList = await query.ToListAsync();
            return miList.Select(mi => mi.AsManageItemDto()).ToList();
        }
        
        [Route(template: "{id}/menu/items/{itemId}")]
        public async Task<ActionResult<RestaurantMenuItemDto>> GetItem([FromRoute] int itemId) {
            var menuItem = await _dbContext.MenuItems.FindAsync(itemId);
            return menuItem  == null ? NotFound() : menuItem .AsManageItemDto();
        }
        
        [Route(template: "{id}/menu/items")]
        [HttpPost]
        public async Task<ActionResult<RestaurantMenuItemDto>> CreateMenuItem([FromBody] RestaurantMenuItemDto miDto) {
            var menuItem = miDto.AsNewMenuItem();
            await _dbContext.MenuItems.AddAsync(menuItem);
            await _dbContext.SaveChangesAsync();
            return menuItem.AsManageItemDto();
        }
        [Route(template: "{id}/menu/items/{itemId}")]
        [HttpPut]
        public async Task<ActionResult<RestaurantMenuItemDto>> UpdateItem([FromRoute] int itemId,
            [FromBody] RestaurantMenuItemDto miDto) {
            var newMenuItem = miDto.AsNewMenuItem();
            await _dbContext.MenuItems.AddAsync(newMenuItem);
            var oldMenuItem = await _dbContext.MenuItems.FindAsync(itemId);
            if (oldMenuItem == null) return NotFound();
            oldMenuItem.UpdateWithRestaurantMenuItemDto(miDto);
            await _dbContext.SaveChangesAsync();
            return newMenuItem.AsManageItemDto();
        }
        [Route(template: "{id}/menu/items/{itemId}")]
        [HttpDelete]
        public async Task<ActionResult<RestaurantMenuItemDto>> DeleteItem( [FromRoute] int itemId) {
            var menuItem = await _dbContext.MenuItems.FindAsync(itemId);
            if (menuItem == null) return NotFound();
            menuItem.MakeMenuItemNonValid();
            await _dbContext.SaveChangesAsync();
            return menuItem.AsManageItemDto();
        }
    }
}
