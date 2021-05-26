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
using pwr_msi.Models.Dto.RestaurantMenu;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [Authorize]
    [ApiController]
    [Route(template: "api/restaurants/")]
    public class RestaurantMenuController : MsiControllerBase {
        private readonly MsiDbContext _dbContext;
        private readonly MenuService _menuService;

        public RestaurantMenuController(MsiDbContext dbContext, MenuService menuService) {
            _dbContext = dbContext;
            _menuService = menuService;
        }

        private ZonedDateTime parseValidAtDate(string validAtString) {
            var parseResult = OffsetDateTimePattern.Rfc3339.Parse(validAtString);
            parseResult.TryGetValue(new OffsetDateTime(), out var validAtDt);
            return validAtDt.InFixedZone();
        }

        [Route(template: "{id}/menu/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<List<MenuCategoryWithItemsDto>>>
            GetMenu([FromRoute] int id, [FromQuery] string validAt) {
            var validAtDt = parseValidAtDate(validAt);
            var mcList = await _menuService.GetMenuFromDb(id, validAtDt);
            return mcList.Select(mc => mc.AsMenuDto()).ToList();
        }

        [Route(template: "{id}/menu/categories/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<List<MenuCategoryDto>>> GetCategories([FromRoute] int id,
            [FromQuery] bool showAll, [FromQuery] string validAt) {
            var validAtDt = parseValidAtDate(validAt);
            var query = _dbContext.MenuCategories.Where(mc => mc.RestaurantId == id);
            if (!showAll) {
                query = query
                .Where(mc => ZonedDateTime.Comparer.Instant.Compare(mc.ValidFrom, validAtDt) <= 0)
                .Where(mc =>
                    mc.ValidUntil == null || ZonedDateTime.Comparer.Instant.Compare(validAtDt, mc.ValidUntil.Value) < 0);
            }

            var mcList = await query.ToListAsync();
            return mcList.Select(mc => mc.AsCategoryDto()).ToList();
        }

        [Route(template: "{id}/menu/bulk/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult> BulkSaveItems([FromRoute] int id,
            [FromBody] BulkSaveDto<MenuItemDto> bulkSaveDto) {
            Debug.Assert(bulkSaveDto.ValidFrom != null, "bulkSaveDto.ValidFrom != null");

            var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
            ActionResult status = Ok();
            await executionStrategy.ExecuteAsync(async () => {
                await using var transaction =
                    await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                var addedItems = bulkSaveDto.Added.Select(miDto => miDto.AsNewMenuItem(id)).ToList();
                var editedItems = bulkSaveDto.Edited.Select(miDto => miDto.AsNewMenuItem(id)).ToList();
                var editedItemIdsToInvalidate = bulkSaveDto.Edited.Select(miDto => miDto.MenuItemId).ToList();
                var deletedItemIdsToInvalidate = bulkSaveDto.Deleted.Select(miDto => miDto.MenuItemId).ToList();

                var itemIdsToInvalidate = editedItemIdsToInvalidate.Concat(deletedItemIdsToInvalidate);
                var itemsToInvalidate = _dbContext.MenuItems.Include(i => i.MenuCategory).Where(i =>
                    i.MenuCategory.RestaurantId == id && itemIdsToInvalidate.Contains(i.MenuItemId));
                var itemCount = await itemsToInvalidate.CountAsync();
                if (itemCount != itemIdsToInvalidate.Count()) {
                    status = BadRequest();
                    return;
                }

                foreach (var item in itemsToInvalidate) {
                    item.Invalidate(bulkSaveDto.ValidFrom);
                }

                addedItems.ForEach(item => item.ValidFrom = bulkSaveDto.ValidFrom.Value);
                editedItems.ForEach(item => item.ValidFrom = bulkSaveDto.ValidFrom.Value);

                await _dbContext.MenuItems.AddRangeAsync(addedItems);
                await _dbContext.MenuItems.AddRangeAsync(editedItems);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await _menuService.InvalidateMenuCache(id);
            });
            return status;
        }

        [Route(template: "{id}/menu/categories/bulk/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult> BulkSaveCategories([FromRoute] int id,
            [FromBody] BulkSaveDto<MenuCategoryDto> bulkSaveDto) {
            Debug.Assert(bulkSaveDto.ValidFrom != null, "bulkSaveDto.ValidFrom != null");

            var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
            ActionResult status = Ok();
            await executionStrategy.ExecuteAsync(async () => {
                await using var transaction =
                    await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                var addedCats = bulkSaveDto.Added.Select(mcDto => mcDto.AsNewMenuCategory(id)).ToList();
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

                addedCats.ForEach(cat => cat.ValidFrom = bulkSaveDto.ValidFrom.Value);
                editedCats.ForEach(cat => cat.ValidFrom = bulkSaveDto.ValidFrom.Value);

                await _dbContext.MenuCategories.AddRangeAsync(addedCats);
                await _dbContext.MenuCategories.AddRangeAsync(editedCats);
                await _dbContext.SaveChangesAsync();

                var categoryIdMap = editedCatIdsToInvalidate.Zip(editedCats.Select(mc => mc.MenuCategoryId))
                    .ToDictionary(kv => kv.First, kv => kv.Second);
                await MigrateMenuItemsByCategory(categoryIdMap, bulkSaveDto.ValidFrom.Value);
                await InvalidateMenuItemsByCategory(deletedCatIdsToInvalidate, bulkSaveDto.ValidFrom.Value);

                await transaction.CommitAsync();
                await _menuService.InvalidateMenuCache(id);
            });
            return status;
        }

        [Route(template: "{id}/menu/latest/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ResultDto<ZonedDateTime?>> MenuLatestVersion([FromRoute] int id) {
            var latest = await _menuService.GetMenuLatestVersionDate(id, Utils.Now());
            return new ResultDto<ZonedDateTime?>(latest);
        }

        /* Old API below - might not work fully */

        [Route(template: "{id}/menu/categories/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult<MenuCategoryDto>> CreateCategory([FromRoute] int id,
            [FromBody] MenuCategoryDto mcDto) {
            var menuCategory = mcDto.AsNewMenuCategory(id);
            await _dbContext.MenuCategories.AddAsync(menuCategory);
            await _dbContext.SaveChangesAsync();
            return menuCategory.AsCategoryDto();
        }

        [Route(template: "{id}/menu/categories/{catId}/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<MenuCategoryDto>> GetCategory([FromRoute] int catId) {
            var category = await _dbContext.MenuCategories.FindAsync(catId);
            return category == null ? NotFound() : category.AsCategoryDto();
        }

        [Route(template: "{id}/menu/categories/{catId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPut]
        public async Task<ActionResult<MenuCategoryDto>> UpdateCategory([FromRoute] int id,
            [FromRoute] int catId,
            [FromBody] MenuCategoryDto mcDto) {
            var menuCategory = await _dbContext.MenuCategories.FindAsync(catId);
            if (menuCategory == null) return NotFound();
            var newMenuCategory = mcDto.AsNewMenuCategory(id);
            await _dbContext.MenuCategories.AddAsync(newMenuCategory);
            menuCategory.UpdateWithMenuCategoryDto(mcDto);
            await _dbContext.SaveChangesAsync();
            await MigrateMenuItemsByCategory(migrationMap(menuCategory.MenuCategoryId, newMenuCategory.MenuCategoryId),
                newMenuCategory.ValidFrom);
            return newMenuCategory.AsCategoryDto();
        }

        [Route(template: "{id}/menu/categories/{catId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpDelete]
        public async Task<ActionResult<MenuCategoryDto>> DeleteCategory([FromRoute] int catId) {
            var menuCategory = await _dbContext.MenuCategories.FindAsync(catId);
            if (menuCategory == null) return NotFound();
            menuCategory.Invalidate();
            Debug.Assert(menuCategory.ValidUntil != null, "menuCategory.ValidUntil != null");
            await InvalidateMenuItemsByCategory(new List<int> {menuCategory.MenuCategoryId},
                menuCategory.ValidUntil.Value);
            await _dbContext.SaveChangesAsync();
            return menuCategory.AsCategoryDto();
        }


        [Route(template: "{id}/menu/items/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<List<MenuItemDto>>> GetItems([FromRoute] int id,
            [FromQuery] bool showAll) {
            IQueryable<MenuItem> query;
            var now = Utils.Now();
            if (showAll) {
                query = _dbContext.MenuItems.Where(mi => (mi.MenuCategory.RestaurantId == id));
            } else {
                query = _dbContext.MenuItems.Where(mi => (mi.MenuCategory.RestaurantId == id)
                                                         && ((mi.ValidUntil == null) ||
                                                             (ZonedDateTime.Comparer.Instant.Compare(
                                                                 (ZonedDateTime) mi.ValidUntil, now) > 0))
                                                         && (ZonedDateTime.Comparer.Instant.Compare(now,
                                                                 mi.ValidFrom) >=
                                                             0));
            }

            var miList = await query.ToListAsync();
            return miList.Select(mi => mi.AsDto()).ToList();
        }

        [Route(template: "{id}/menu/items/{itemId}/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<MenuItemDto>> GetItem([FromRoute] int itemId) {
            var menuItem = await _dbContext.MenuItems.FindAsync(itemId);
            // ReSharper disable once MergeConditionalExpression
            return menuItem == null ? NotFound() : menuItem.AsDto();
        }

        [Route(template: "{id}/menu/items/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem([FromRoute] int id, [FromBody] MenuItemDto miDto) {
            var menuItem = miDto.AsNewMenuItem(id);
            await _dbContext.MenuItems.AddAsync(menuItem);
            await _dbContext.SaveChangesAsync();
            return menuItem.AsDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPut]
        public async Task<ActionResult<MenuItemDto>> UpdateItem([FromRoute] int id, [FromRoute] int itemId,
            [FromBody] MenuItemDto miDto) {
            var newMenuItem = miDto.AsNewMenuItem(id);
            await _dbContext.MenuItems.AddAsync(newMenuItem);
            var oldMenuItem = await _dbContext.MenuItems.FindAsync(itemId);
            if (oldMenuItem == null) return NotFound();
            oldMenuItem.UpdateWithRestaurantMenuItemDto(miDto);
            await _dbContext.SaveChangesAsync();
            await MigrateMenuOptionListsByItem(migrationMap(oldMenuItem.MenuItemId, newMenuItem.MenuItemId));
            return newMenuItem.AsDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpDelete]
        public async Task<ActionResult<MenuItemDto>> DeleteItem([FromRoute] int itemId) {
            var menuItem = await _dbContext.MenuItems.FindAsync(itemId);
            if (menuItem == null) return NotFound();
            menuItem.Invalidate();
            await _dbContext.SaveChangesAsync();
            return menuItem.AsDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/options/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult<MenuItemOptionListDto>> CreateMenuItemOptionList(
            [FromBody] MenuItemOptionListDto miolDto, [FromRoute] int itemId) {
            var optionList = miolDto.AsNewMenuItemOptionList();
            optionList.MenuItemId = itemId;
            await _dbContext.MenuItemOptionLists.AddAsync(optionList);
            await _dbContext.SaveChangesAsync();
            return optionList.AsDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpDelete]
        public async Task<ActionResult<MenuItemOptionListDto>> DeleteOptionList([FromRoute] int listId) {
            var optionList = await _dbContext.MenuItemOptionLists.FindAsync(listId);
            if (optionList == null) return NotFound();
            var query = _dbContext.MenuItemOptionItems.Where(mioi => mioi.MenuItemOptionListId == listId);
            var optionItems = await query.ToListAsync();
            foreach (var item in optionItems) {
                _dbContext.MenuItemOptionItems.Remove(item);
            }

            _dbContext.MenuItemOptionLists.Remove(optionList);
            await _dbContext.SaveChangesAsync();
            return optionList.AsDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPost]
        public async Task<ActionResult<MenuItemOptionItemDto>> CreateMenuItemOptionItem(
            [FromBody] MenuItemOptionItemDto mioiDto, [FromRoute] int listId) {
            var optionItem = mioiDto.AsNewMenuItemOptionItem();
            optionItem.MenuItemOptionListId = listId;
            await _dbContext.MenuItemOptionItems.AddAsync(optionItem);
            await _dbContext.SaveChangesAsync();
            return optionItem.AsDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/{oId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpDelete]
        public async Task<ActionResult<MenuItemOptionItemDto>> DeleteOptionItem([FromRoute] int oId) {
            var optionItem = await _dbContext.MenuItemOptionItems.FindAsync(oId);
            if (optionItem == null) return NotFound();
            _dbContext.MenuItemOptionItems.Remove(optionItem);
            await _dbContext.SaveChangesAsync();
            return optionItem.AsDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/{oId}/")]
        [ManageRestaurantAuthorize("id")]
        [HttpPut]
        public async Task<ActionResult<MenuItemOptionItemDto>> UpdateOptionItem([FromRoute] int itemId,
            [FromBody] MenuItemOptionItemDto mioiDto) {
            var newOptionItem = mioiDto.AsNewMenuItemOptionItem();
            newOptionItem.MenuItemOptionListId = mioiDto.MenuItemOptionListId;
            await _dbContext.MenuItemOptionItems.AddAsync(newOptionItem);
            var oldOptionItem = await _dbContext.MenuItemOptionItems.FindAsync(itemId);
            if (oldOptionItem == null) return NotFound();
            oldOptionItem.UpdateWithRestaurantMenuItemOptionItemDto(mioiDto);
            await _dbContext.SaveChangesAsync();
            return newOptionItem.AsDto();
        }

        [Route(template: "{id}/menu/items/{itemId}/options/{listId}/{oId}/")]
        [ManageRestaurantAuthorize("id")]
        public async Task<ActionResult<MenuItemOptionItemDto>> GetOptionItem([FromRoute] int oId) {
            var optionItem = await _dbContext.MenuItemOptionItems.FindAsync(oId);
            return optionItem == null ? NotFound() : optionItem.AsDto();
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
