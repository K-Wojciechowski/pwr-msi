#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NodaTime;
using pwr_msi.Models;
using pwr_msi.Models.Dto;
using pwr_msi.Models.Dto.RestaurantMenu;
using pwr_msi.Serialization;

namespace pwr_msi.Services {
    public class MenuService {
        private readonly MsiDbContext _dbContext;
        private readonly IDistributedCache _cache;

        public MenuService(MsiDbContext dbContext, IDistributedCache cache) {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<(List<MenuCategory> mcs, List<MenuItem> mis)> GetCategoriesItems(int restaurantId,
            ZonedDateTime validAt) {
            var mcQuery = _dbContext.MenuCategories
                .Where(mc => mc.RestaurantId == restaurantId)
                .Where(mc => ZonedDateTime.Comparer.Instant.Compare(mc.ValidFrom, validAt) <= 0)
                .Where(mc =>
                    mc.ValidUntil == null || ZonedDateTime.Comparer.Instant.Compare(validAt, mc.ValidUntil.Value) < 0);

            var mcIds = await mcQuery.Select(mc => mc.MenuCategoryId).ToListAsync();

            var miQuery = _dbContext.MenuItems
                .Include(mi => mi.Options)
                .Include("Options.Items")
                .Where(mi => mcIds.Contains(mi.MenuCategoryId))
                .Where(mi => ZonedDateTime.Comparer.Instant.Compare(mi.ValidFrom, validAt) <= 0)
                .Where(mi =>
                    mi.ValidUntil == null || ZonedDateTime.Comparer.Instant.Compare(validAt, mi.ValidUntil.Value) < 0);

            var mcs = await mcQuery.ToListAsync();
            var mis = await miQuery.ToListAsync();
            return (mcs, mis);
        }

        public async Task<List<MenuCategory>> GetMenuFromDb(int restaurantId, ZonedDateTime validAt) {
            var (mcs, mis) = await GetCategoriesItems(restaurantId, validAt);
            var itemsByCategory = mis.GroupBy(mi => mi.MenuCategoryId).ToDictionary(g => g.Key, g => g.ToList());
            mcs.ForEach(mc =>
                mc.Items = itemsByCategory.GetValueOrDefault(mc.MenuCategoryId, new List<MenuItem>())
            );
            return mcs;
        }

        public async Task<IEnumerable<ZonedDateTime>> GetMenuValidityDates(int restaurantId, ZonedDateTime validAt) {
            var mcQuery = _dbContext.MenuCategories
                .Where(mc => mc.RestaurantId == restaurantId)
                .Where(mc => mc.ValidUntil == null || ZonedDateTime.Comparer.Instant.Compare(validAt, mc.ValidUntil.Value) <= 0);

            var mcIds = await mcQuery.Select(mc => mc.MenuCategoryId).ToListAsync();
            var miQuery = _dbContext.MenuItems
                .Where(mi => mcIds.Contains(mi.MenuCategoryId))
                .Where(mi => mi.ValidUntil == null || ZonedDateTime.Comparer.Instant.Compare(validAt, mi.ValidUntil.Value) <= 0);

            var mcs = await mcQuery.ToListAsync();
            var mis = await miQuery.ToListAsync();
            var mcDates = mcs.SelectMany(mc => ValidDateList(mc.ValidFrom, mc.ValidUntil));
            var miDates = mis.SelectMany(mi => ValidDateList(mi.ValidFrom, mi.ValidUntil));
            return mcDates.Concat(miDates).Where(date => ZonedDateTime.Comparer.Instant.Compare(date, validAt) > 0);
        }

        public async Task<ZonedDateTime?> GetMenuExpirationDate(int restaurantId, ZonedDateTime validAt) {
            var validityDates = (await GetMenuValidityDates(restaurantId, validAt)).ToList();
            return validityDates.Any() ? validityDates.Min(dt => dt.ToInstant()).InUtc() : null;
        }

        public async Task<ZonedDateTime?> GetMenuLatestVersionDate(int restaurantId, ZonedDateTime validAt) {
            var validityDates = (await GetMenuValidityDates(restaurantId, validAt)).ToList();
            return validityDates.Any() ? validityDates.Max(dt => dt.ToInstant()).InUtc() : null;
        }

        private static IEnumerable<ZonedDateTime> ValidDateList(ZonedDateTime first, ZonedDateTime? second) {
            return second.HasValue ? new List<ZonedDateTime> {first, second.Value} : new List<ZonedDateTime> {first};
        }

        public async Task<List<MenuCategoryWithItemsDto>> GetMenuFromDbAndCache(int restaurantId, ZonedDateTime validAt) {
            var dbMenu = await GetMenuFromDb(restaurantId, validAt);
            var menu = dbMenu.Select(mc => mc.AsMenuDto()).ToList();
            var expirationDate = await GetMenuExpirationDate(restaurantId, validAt);

            if (menu.Count > 0) {
                var cacheEntry = new MenuCacheEntry(menu, validAt, expirationDate);
                var cacheEntryString =
                    JsonConvert.SerializeObject(cacheEntry, MsiSerializerSettings.jsonSerializerSettings);
                var options = new DistributedCacheEntryOptions();
                if (expirationDate != null) {
                    options.SetAbsoluteExpiration(expirationDate.Value.ToDateTimeOffset());
                }

                await _cache.SetStringAsync("menu:" + restaurantId, cacheEntryString, options);
            }

            return menu;
        }

        public async Task<List<MenuCategoryWithItemsDto>> GetMenuFromCache(int restaurantId,
            ZonedDateTime validAt) {
            var cacheEntryString = await _cache.GetStringAsync("menu:" + restaurantId);
            if (cacheEntryString == null) {
                return await GetMenuFromDbAndCache(restaurantId, validAt);
            }

            var cacheEntry = JsonConvert.DeserializeObject<MenuCacheEntry>(cacheEntryString, MsiSerializerSettings.jsonSerializerSettings);
            if (ZonedDateTime.Comparer.Instant.Compare(validAt, cacheEntry.validAt) < 0) {
                // Going backwards, avoid cache
            var dbMenu = await GetMenuFromDb(restaurantId, validAt);
            var menu = dbMenu.Select(mc => mc.AsMenuDto()).ToList();
            return menu;
            }

            if (cacheEntry.expiresAt != null &&
                ZonedDateTime.Comparer.Instant.Compare(validAt, cacheEntry.expiresAt.Value) > 0) {
                // Entry has expired, update cache
                return await GetMenuFromDbAndCache(restaurantId, validAt);
            }

            return cacheEntry.menuCategories;

        }

        public async Task InvalidateMenuCache(int restaurantId) {
            await _cache.RemoveAsync("menu:" + restaurantId);
        }
    }
}
