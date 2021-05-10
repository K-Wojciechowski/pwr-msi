using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.RestaurantManagement;

namespace pwr_msi.Controllers {
    [Authorize]
    [ApiController]
    [Route(template: "api/restaurants/")]
    public class RestaurantMenuController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;

        public RestaurantMenuController(MsiDbContext dbContext) {
            _dbContext = dbContext;
        }

        private ZonedDateTime parseValidAtDate(string validAtString) {
            var parseResult = OffsetDateTimePattern.Rfc3339.Parse(validAtString);
            parseResult.TryGetValue(new OffsetDateTime(), out var validAtDt);
            return validAtDt.InFixedZone();
        }

        [Route(template: "{id}/menu/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<List<RestaurantMenuDto>>>
            GetMenu([FromRoute] int id, [FromQuery] string validAt) {
            var validAtDt = parseValidAtDate(validAt);
            var query = _dbContext.MenuCategories.Where(mc => (mc.RestaurantId == id)
                                                              && ((mc.ValidUntil == null) ||
                                                                  (ZonedDateTime.Comparer.Instant.Compare(
                                                                      (ZonedDateTime) mc.ValidUntil, validAtDt) > 0))
                                                              && (ZonedDateTime.Comparer.Instant.Compare(validAtDt,
                                                                  mc.ValidFrom) >= 0));
            var mcList = await query.ToListAsync();
            return mcList.Select(mc => mc.AsManageMenuDto()).ToList();
        }

        [Route(template: "{id}/menu/categories/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<List<RestaurantMenuCategoryDto>>> GetCategories([FromRoute] int id,
            [FromQuery] bool showAll, [FromQuery] string validAt) {
            var validAtDt = parseValidAtDate(validAt);
            IQueryable<MenuCategory> query;

            if (showAll) {
                query = _dbContext.MenuCategories.Where(mc => (mc.RestaurantId == id));
            } else {
                query = _dbContext.MenuCategories.Where(mc => (mc.RestaurantId == id)
                                                              && ((mc.ValidUntil == null) ||
                                                                  (ZonedDateTime.Comparer.Instant.Compare(
                                                                      (ZonedDateTime) mc.ValidUntil, validAtDt) > 0))
                                                              && (ZonedDateTime.Comparer.Instant.Compare(
                                                                  validAtDt, mc.ValidFrom) >= 0));
            }

            var mcList = await query.ToListAsync();
            return mcList.Select(mc => mc.AsManageCategoryDto()).ToList();
        }


        [Route(template: "{id}/menu/categories/bulk/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult> BulkSaveCategories([FromRoute] int id,
            [FromBody] BulkSaveDto<RestaurantMenuCategoryDto> bulkSaveDto) {
            Debug.Assert(bulkSaveDto.ValidFrom != null, "bulkSaveDto.ValidFrom != null");

            var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
            ActionResult status = Ok();
            await executionStrategy.ExecuteAsync(async () => {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var addedCats = bulkSaveDto.Added.Select(mcDto => mcDto.AsNewMenuCategory(id));
            var editedCats = bulkSaveDto.Edited.Select(mcDto => mcDto.AsNewMenuCategory(id)).ToList();
            var editedCatIdsToInvalidate = bulkSaveDto.Edited.Select(mcDto => mcDto.MenuCategoryId).ToList();
            var deletedCatIdsToInvalidate = bulkSaveDto.Deleted.Select(mcDto => mcDto.MenuCategoryId).ToList();

            var catIdsToInvalidate = editedCatIdsToInvalidate.Concat(deletedCatIdsToInvalidate);
            var catsToInvalidate = _dbContext.MenuCategories.Where(c =>
                c.RestaurantId == id && catIdsToInvalidate.Contains(c.MenuCategoryId));
            var catCount = await catsToInvalidate.CountAsync();
            if (catCount != catIdsToInvalidate.Count()) {
                status = BadRequest();
                return;
            }

            foreach (var cat in catsToInvalidate) {
                cat.Invalidate(bulkSaveDto.ValidFrom);
            }

            await _dbContext.MenuCategories.AddRangeAsync(addedCats);
            await _dbContext.MenuCategories.AddRangeAsync(editedCats);
            await _dbContext.SaveChangesAsync();

            var categoryIdMap = editedCatIdsToInvalidate.Zip(editedCats.Select(mc => mc.MenuCategoryId))
                .ToDictionary(kv => kv.First, kv => kv.Second);
            await MigrateMenuItemsByCategory(categoryIdMap, bulkSaveDto.ValidFrom.Value);
            await InvalidateMenuItemsByCategory(deletedCatIdsToInvalidate, bulkSaveDto.ValidFrom.Value);

            await transaction.CommitAsync();

            });
            return status;
        }

        [Route(template: "{id}/menu/categories/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> CreateCategory([FromRoute] int id,
            [FromBody] RestaurantMenuCategoryDto mcDto) {
            var menuCategory = mcDto.AsNewMenuCategory(id);
            await _dbContext.MenuCategories.AddAsync(menuCategory);
            await _dbContext.SaveChangesAsync();
            return menuCategory.AsManageCategoryDto();
        }

        [Route(template: "{id}/menu/categories/{catId}/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> GetCategory([FromRoute] int catId) {
            var category = await _dbContext.MenuCategories.FindAsync(catId);
            return category == null ? NotFound() : category.AsManageCategoryDto();
        }

        [Route(template: "{id}/menu/categories/{catId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPut]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> UpdateCategory([FromRoute] int id,
            [FromRoute] int catId,
            [FromBody] RestaurantMenuCategoryDto mcDto) {
            var menuCategory = await _dbContext.MenuCategories.FindAsync(catId);
            if (menuCategory == null) return NotFound();
            var newMenuCategory = mcDto.AsNewMenuCategory(id);
            await _dbContext.MenuCategories.AddAsync(newMenuCategory);
            menuCategory.UpdateWithRestaurantMenuCategoryDto(mcDto);
            await _dbContext.SaveChangesAsync();
            await MigrateMenuItemsByCategory(migrationMap(menuCategory.MenuCategoryId, newMenuCategory.MenuCategoryId),
                newMenuCategory.ValidFrom);
            return newMenuCategory.AsManageCategoryDto();
        }

        [Route(template: "{id}/menu/categories/{catId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpDelete]
        public async Task<ActionResult<RestaurantMenuCategoryDto>> DeleteCategory([FromRoute] int catId) {
            var menuCategory = await _dbContext.MenuCategories.FindAsync(catId);
            if (menuCategory == null) return NotFound();
            menuCategory.Invalidate();
            Debug.Assert(menuCategory.ValidUntil != null, "menuCategory.ValidUntil != null");
            await InvalidateMenuItemsByCategory(new List<int> {menuCategory.MenuCategoryId},
                menuCategory.ValidUntil.Value);
            await _dbContext.SaveChangesAsync();
            return menuCategory.AsManageCategoryDto();
        }


        [Route(template: "{id}/menu/items/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<List<RestaurantMenuItemDto>>> GetItems([FromRoute] int id,
            [FromQuery] bool showAll) {
            IQueryable<MenuItem> query;
            ZonedDateTime now = new ZonedDateTime();
            if (showAll) {
                query = _dbContext.MenuItems.Where(mi => (mi.MenuCategory.RestaurantId == id));
            } else {
                query = _dbContext.MenuItems.Where(mi => (mi.MenuCategory.RestaurantId == id)
                                                         && ((mi.ValidUntil == null) ||
                                                             (ZonedDateTime.Comparer.Instant.Compare(
                                                                 (ZonedDateTime) mi.ValidUntil, now) > 0))
                                                         && (ZonedDateTime.Comparer.Instant.Compare(now, mi.ValidFrom) >=
                                                             0));
            }

            var miList = await query.ToListAsync();
            return miList.Select(mi => mi.AsManageItemDto()).ToList();
        }

        [Route(template: "{id}/menu/items/{itemId}/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<RestaurantMenuItemDto>> GetItem([FromRoute] int itemId) {
            var menuItem = await _dbContext.MenuItems.FindAsync(itemId);
            // ReSharper disable once MergeConditionalExpression
            return menuItem == null ? NotFound() : menuItem.AsManageItemDto();
        }

        [Route(template: "{id}/menu/items/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult<RestaurantMenuItemDto>> CreateMenuItem([FromBody] RestaurantMenuItemDto miDto) {
            var menuItem = miDto.AsNewMenuItem();
            await _dbContext.MenuItems.AddAsync(menuItem);
            await _dbContext.SaveChangesAsync();
            return menuItem.AsManageItemDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPut]
        public async Task<ActionResult<RestaurantMenuItemDto>> UpdateItem([FromRoute] int itemId,
            [FromBody] RestaurantMenuItemDto miDto) {
            var newMenuItem = miDto.AsNewMenuItem();
            await _dbContext.MenuItems.AddAsync(newMenuItem);
            var oldMenuItem = await _dbContext.MenuItems.FindAsync(itemId);
            if (oldMenuItem == null) return NotFound();
            oldMenuItem.UpdateWithRestaurantMenuItemDto(miDto);
            await _dbContext.SaveChangesAsync();
            await MigrateMenuOptionListsByItem(migrationMap(oldMenuItem.MenuItemId, newMenuItem.MenuItemId));
            return newMenuItem.AsManageItemDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpDelete]
        public async Task<ActionResult<RestaurantMenuItemDto>> DeleteItem([FromRoute] int itemId) {
            var menuItem = await _dbContext.MenuItems.FindAsync(itemId);
            if (menuItem == null) return NotFound();
            menuItem.Invalidate();
            await _dbContext.SaveChangesAsync();
            return menuItem.AsManageItemDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/options/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult<RestaurantMenuItemOptionListDto>> CreateMenuItemOptionList(
            [FromBody] RestaurantMenuItemOptionListDto miolDto, [FromRoute] int itemId) {
            var optionList = miolDto.AsNewMenuItemOptionList(itemId);
            await _dbContext.MenuItemOptionLists.AddAsync(optionList);
            await _dbContext.SaveChangesAsync();
            return optionList.AsManageOptionListDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/")]
        [ManageRestaurantAuthorize("id")]
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

        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult<RestaurantMenuItemOptionItemDto>> CreateMenuItemOptionItem(
            [FromBody] RestaurantMenuItemOptionItemDto mioiDto, [FromRoute] int listId) {
            var optionItem = mioiDto.AsNewMenuItemOptionItem(listId);
            await _dbContext.MenuItemOptionItems.AddAsync(optionItem);
            await _dbContext.SaveChangesAsync();
            return optionItem.AsManageOptionItemDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/{oId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpDelete]
        public async Task<ActionResult<RestaurantMenuItemOptionItemDto>> DeleteOptionItem([FromRoute] int oId) {
            var optionItem = await _dbContext.MenuItemOptionItems.FindAsync(oId);
            if (optionItem == null) return NotFound();
            _dbContext.MenuItemOptionItems.Remove(optionItem);
            await _dbContext.SaveChangesAsync();
            return optionItem.AsManageOptionItemDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/{oId}/")]
        [ManageRestaurantAuthorize("id")]
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

        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/{oId}/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<RestaurantMenuItemOptionItemDto>> GetOptionItem([FromRoute] int oId) {
            var optionItem = await _dbContext.MenuItemOptionItems.FindAsync(oId);
            return optionItem == null ? NotFound() : optionItem.AsManageOptionItemDto();
        }

        private async Task MigrateMenuItemsByCategory(Dictionary<int, int> categoryIdMap, ZonedDateTime validFrom) {
            var oldCategoryIds = categoryIdMap.Keys.ToHashSet();
            var itemsToUpdate = await _dbContext.MenuItems.Where(mi => oldCategoryIds.Contains(mi.MenuCategoryId))
                .ToListAsync();
            var createdMenuItems = itemsToUpdate
                .Select(mi => mi.CreateNewWithCategory(categoryIdMap[mi.MenuCategoryId], validFrom)).ToList();
            await _dbContext.AddRangeAsync(createdMenuItems);
            await _dbContext.SaveChangesAsync();
            var itemIdMap = itemsToUpdate.Zip(createdMenuItems)
                .ToDictionary(kv => kv.First.MenuItemId, kv => kv.Second.MenuItemId);
            await MigrateMenuOptionListsByItem(itemIdMap);
        }

        private async Task MigrateMenuOptionListsByItem(Dictionary<int, int> itemIdMap) {
            var oldItemIds = itemIdMap.Keys.ToHashSet();
            var optionListsToUpdate = await _dbContext.MenuItemOptionLists
                .Where(ol => oldItemIds.Contains(ol.MenuItemId))
                .ToListAsync();
            var createdOptionLists = optionListsToUpdate.Select(ol => ol.CreateNewWithItem(itemIdMap[ol.MenuItemId]))
                .ToList();
            await _dbContext.AddRangeAsync(createdOptionLists);
            await _dbContext.SaveChangesAsync();
            var optionListIdMap = optionListsToUpdate.Zip(createdOptionLists)
                .ToDictionary(kv => kv.First.MenuItemOptionListId, kv => kv.Second.MenuItemOptionListId);
            await MigrateMenuOptionItemsByOptionList(optionListIdMap);
        }


        private async Task MigrateMenuOptionItemsByOptionList(Dictionary<int, int> optionListIdMap) {
            var oldOptionListIds = optionListIdMap.Keys.ToHashSet();
            var optionItemsToUpdate = await _dbContext.MenuItemOptionItems
                .Where(oi => oldOptionListIds.Contains(oi.MenuItemOptionListId))
                .ToListAsync();
            var createdOptionItems = optionItemsToUpdate
                .Select(oi => oi.CreateNewWithList(optionListIdMap[oi.MenuItemOptionListId]))
                .ToList();
            await _dbContext.AddRangeAsync(createdOptionItems);
            await _dbContext.SaveChangesAsync();
        }

        private async Task InvalidateMenuItemsByCategory(ICollection<int> categoryIds, ZonedDateTime validUntil) {
            var items = await _dbContext.MenuItems.Where(mi => categoryIds.Contains(mi.MenuCategoryId)).ToListAsync();
            items.ForEach(mi => mi.Invalidate(validUntil));
            await _dbContext.SaveChangesAsync();
        }

        private static Dictionary<int, int> migrationMap(int oldId, int newId) => new() {{oldId, newId}};
    }
}
