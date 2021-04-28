using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using pwr_msi.Models;
using pwr_msi.Models.Dto;

namespace pwr_msi.Controllers {
    [Authorize]
    [AdminAuthorize]
    [ApiController]
    [Route(template: "api/restaurant/")]
    public class RestaurantMenuController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        
        public RestaurantMenuController(MsiDbContext dbContext) {
            _dbContext = dbContext;
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/")]
        public async Task<ActionResult<List<RestaurantMenuDto>>> GetMenu([FromRoute] int id) {
            ZonedDateTime now = new ZonedDateTime();
            var query = _dbContext.MenuCategories.Where(mc =>( mc.RestaurantId == id)
                                                             && ((mc.ValidUntil == null) || (ZonedDateTime.Comparer.Local.Compare((ZonedDateTime)mc.ValidUntil, now) >= 0))
                                                                 && (ZonedDateTime.Comparer.Local.Compare(now, mc.ValidFrom) > 0));
            var mcList = await query.ToListAsync();
            return mcList.Select(mc => mc.AsManageMenuDto()).ToList();
        }

        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/categories/")]
        public async Task<ActionResult<List<RestaurantMenuCategoryDto>>> GetCategories([FromRoute] int id, [FromQuery] bool showAll) {
            ZonedDateTime now = new ZonedDateTime();
            IQueryable<MenuCategory> query;
            if (showAll) {
                query = _dbContext.MenuCategories.Where(mc => (mc.RestaurantId == id));
            } else {
                query = _dbContext.MenuCategories.Where(mc => (mc.RestaurantId == id) 
                                                              && ((mc.ValidUntil == null) || (ZonedDateTime.Comparer.Local.Compare((ZonedDateTime)mc.ValidUntil, now) >= 0))
                                                                  && (ZonedDateTime.Comparer.Local.Compare(
                                                                      now, mc.ValidFrom) > 0));
            }
            var mcList = await query.ToListAsync();
            return mcList.Select(mc => mc.AsManageCategoryDto()).ToList();
        }

        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/categories/")]
        [HttpPost]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> CreateCategory([FromRoute] int id,
            [FromBody] RestaurantMenuCategoryDto mcDto) {
            var menuCategory = mcDto.AsNewMenuCategory(id);
            await _dbContext.MenuCategories.AddAsync(menuCategory);
            await _dbContext.SaveChangesAsync();
            return menuCategory.AsManageCategoryDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/categories/{catId}/")]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> GetCategory([FromRoute] int catId, [FromRoute] int id) {
            var category = await _dbContext.MenuCategories.Where(cat => cat.RestaurantId == id && cat.MenuCategoryId == catId).FirstOrDefaultAsync();
            return category == null ? NotFound() : category.AsManageCategoryDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/categories/{catId}/")]
        [HttpPut]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> UpdateCategory([FromRoute] int id, [FromRoute] int catId,
            [FromBody] RestaurantMenuCategoryDto mcDto) {
            var menuCategory = await _dbContext.MenuCategories.Where(cat => cat.RestaurantId == id && cat.MenuCategoryId == catId).FirstOrDefaultAsync();
            var newMenuCategory = mcDto.AsNewMenuCategory(id);
            await _dbContext.MenuCategories.AddAsync(newMenuCategory);
            if (menuCategory == null) return NotFound();
            menuCategory.UpdateWithRestaurantMenuCategoryDto(mcDto);
            await _dbContext.SaveChangesAsync();
            return newMenuCategory.AsManageCategoryDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/categories/{catId}/")]
        [HttpDelete]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> DeleteCategory( [FromRoute] int catId, [FromRoute] int id) {
            var menuCategory = await _dbContext.MenuCategories.Where(cat => cat.RestaurantId == id && cat.MenuCategoryId == catId).FirstOrDefaultAsync();
            if (menuCategory == null) return NotFound();
            menuCategory.MakeMenuCategoryNonValid();
            await _dbContext.SaveChangesAsync();
            return menuCategory.AsManageCategoryDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/")]
        public async Task<ActionResult<List<RestaurantMenuItemDto>>> GetItems([FromRoute] int id, [FromQuery] bool showAll) {
            IQueryable<MenuItem> query;
            ZonedDateTime now = new ZonedDateTime();
            if (showAll) {
                query = _dbContext.MenuItems.Where(mi => (mi.MenuCategory.RestaurantId == id));
            } 
            else 
            {
                query = _dbContext.MenuItems.Where(mi => (mi.MenuCategory.RestaurantId == id) 
                                                             && ((mi.ValidUntil == null) || (ZonedDateTime.Comparer.Local.Compare((ZonedDateTime)mi.ValidUntil, now) >= 0))
                                                                 && (ZonedDateTime.Comparer.Local.Compare(now, mi.ValidFrom) > 0));
            }
            
            var miList = await query.ToListAsync();
            return miList.Select(mi => mi.AsManageItemDto()).ToList();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/{itemId}/")]
        public async Task<ActionResult<RestaurantMenuItemDto>> GetItem([FromRoute] int itemId, [FromRoute] int id) {
            var menuItem = await _dbContext.MenuItems.Where(mi => mi.MenuCategory.RestaurantId == id && mi.MenuItemId == itemId).FirstOrDefaultAsync();
            return menuItem  == null ? NotFound() : menuItem .AsManageItemDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/")]
        [HttpPost]
        public async Task<ActionResult<RestaurantMenuItemDto>> CreateMenuItem([FromBody] RestaurantMenuItemDto miDto) {
            var menuItem = miDto.AsNewMenuItem();
            await _dbContext.MenuItems.AddAsync(menuItem);
            await _dbContext.SaveChangesAsync();
            return menuItem.AsManageItemDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/{itemId}/")]
        [HttpPut]
        public async Task<ActionResult<RestaurantMenuItemDto>> UpdateItem([FromRoute] int itemId, [FromRoute] int id,
            [FromBody] RestaurantMenuItemDto miDto) {
            var newMenuItem = miDto.AsNewMenuItem();
            await _dbContext.MenuItems.AddAsync(newMenuItem);
            var oldMenuItem = await _dbContext.MenuItems.Where(mi => mi.MenuCategory.RestaurantId == id && mi.MenuItemId == itemId).FirstOrDefaultAsync();
            if (oldMenuItem == null) return NotFound();
            oldMenuItem.UpdateWithRestaurantMenuItemDto(miDto);
            await _dbContext.SaveChangesAsync();
            return newMenuItem.AsManageItemDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/{itemId}/")]
        [HttpDelete]
        public async Task<ActionResult<RestaurantMenuItemDto>> DeleteItem([FromRoute] int itemId, [FromRoute] int id) {
            var menuItem = await _dbContext.MenuItems.Where(mi => mi.MenuCategory.RestaurantId == id && mi.MenuItemId == itemId).FirstOrDefaultAsync();
            if (menuItem == null) return NotFound();
            menuItem.MakeMenuItemNonValid();
            await _dbContext.SaveChangesAsync();
            return menuItem.AsManageItemDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/{itemId}/options/")]
        [HttpPost]
        public async Task<ActionResult<RestaurantMenuItemOptionListDto>> CreateMenuItemOptionList([FromBody] RestaurantMenuItemOptionListDto miolDto, [FromRoute] int itemId) {
            var optionList = miolDto.AsNewMenuItemOptionList(itemId);
            await _dbContext.MenuItemOptionLists.AddAsync(optionList);
            await _dbContext.SaveChangesAsync();
            return optionList.AsManageOptionListDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/")]
        [HttpDelete]
        public async Task<ActionResult<RestaurantMenuItemOptionListDto>> DeleteOptionList([FromRoute] int listId) {
            var optionList = await _dbContext.MenuItemOptionLists.FindAsync(listId);
            if (optionList == null) return NotFound();
            var query = _dbContext.MenuItemOptionItems.Where(mioi => mioi.MenuItemOptionListId == listId);
            var optionItems = await query.ToListAsync();
            foreach (var item in optionItems) {
                _dbContext.MenuItemOptionItems.Remove(item);
            }
            _dbContext.MenuItemOptionLists.Remove(optionList);
            await _dbContext.SaveChangesAsync();
            return optionList.AsManageOptionListDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/")]
        [HttpPost]
        public async Task<ActionResult<RestaurantMenuItemOptionItemDto>> CreateMenuItemOptionItem([FromBody] RestaurantMenuItemOptionItemDto mioiDto, [FromRoute] int listId) {
            var optionItem = mioiDto.AsNewMenuItemOptionItem(listId);
            await _dbContext.MenuItemOptionItems.AddAsync(optionItem);
            await _dbContext.SaveChangesAsync();
            return optionItem.AsManageOptionItemDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/{oId}/")]
        [HttpDelete]
        public async Task<ActionResult<RestaurantMenuItemOptionItemDto>> DeleteOptionItem([FromRoute] int oId) {
            var optionItem = await _dbContext.MenuItemOptionItems.FindAsync(oId);
            if (optionItem == null) return NotFound();
            _dbContext.MenuItemOptionItems.Remove(optionItem);
            await _dbContext.SaveChangesAsync();
            return optionItem.AsManageOptionItemDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/{oId}/")]
        [HttpPut]
        public async Task<ActionResult<RestaurantMenuItemOptionItemDto>> UpdateOptionItem([FromRoute] int itemId,
            [FromBody] RestaurantMenuItemOptionItemDto mioiDto) {
            var newOptionItem = mioiDto.AsNewMenuItemOptionItem(mioiDto.MenuItemOptionListId);
            await _dbContext.MenuItemOptionItems.AddAsync(newOptionItem);
            var oldOptionItem = await _dbContext.MenuItemOptionItems.FindAsync(itemId);
            if (oldOptionItem == null) return NotFound();
            oldOptionItem.UpdateWithRestaurantMenuItemOptionItemDto(mioiDto);
            await _dbContext.SaveChangesAsync();
            return newOptionItem.AsManageOptionItemDto();
        }
        
        [ManageRestaurantAuthorize("id")]
        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/{oId}/")]
        public async Task<ActionResult<RestaurantMenuItemOptionItemDto>> GetOptionItem([FromRoute] int oId) {
            var optionItem = await _dbContext.MenuItemOptionItems.FindAsync(oId);
            return optionItem  == null ? NotFound() : optionItem .AsManageOptionItemDto();
        }
    }
}
