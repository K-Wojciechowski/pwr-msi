#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using pwr_msi.Models.Dto;

namespace pwr_msi {
    public static class Utils {
        public static int? TryParseInt(string? s) {
            var res = int.TryParse(s, out var i);
            return res ? i : null;
        }

        public static ZonedDateTime Now() {
            var now = SystemClock.Instance.GetCurrentInstant();
            return now.InUtc();
        }

        public static OffsetDateTime OffsetNow() {
            return Now().ToOffsetDateTime();
        }

        public static async Task<Page<TO>> Paginate<TD, TO>(IQueryable<TD> queryable, int pageRaw,
            Func<TD, TO> converter) where TD : class {
            var itemCount = await queryable.CountAsync();
            var maxPage = Math.Max(1, (int) Math.Ceiling(a: itemCount / (double) Constants.PageSize));
            var page = pageRaw;
            if (page <= 0) page = 1;
            if (page > maxPage) page = maxPage;
            var items = await queryable.Skip(count: (page - 1) * Constants.PageSize).Take(Constants.PageSize)
                .ToListAsync();
            return new Page<TO> {Items = items.Select(converter), MaxPage = maxPage, ItemCount = itemCount, PageNumber = page};
        }

        public static async Task<Page<TO>> PaginateAsync<TD, TO>(IQueryable<TD> queryable, int pageRaw,
            Func<TD, Task<TO>> converter) where TD : class {
            var itemCount = await queryable.CountAsync();
            var maxPage = Math.Max(1, (int) Math.Ceiling(a: itemCount / (double) Constants.PageSize));
            var page = pageRaw;
            if (page <= 0) page = 1;
            if (page > maxPage) page = maxPage;
            var items = await queryable.Skip(count: (page - 1) * Constants.PageSize).Take(Constants.PageSize)
                .ToListAsync();
            var convertedItems = new List<TO>();
            foreach (var item in items) {
                convertedItems.Add(await converter(item));
            }
            return new Page<TO> {Items = convertedItems, MaxPage = maxPage, ItemCount = itemCount, PageNumber = page};
        }

        public static async Task<Page<TO>> PaginateAsyncNullable<TD, TO>(IQueryable<TD> queryable, int pageRaw,
            Func<TD, Task<TO?>> converter) where TD : class {
            var pageWithNulls = await PaginateAsync(queryable, pageRaw, converter);
            var cleanedItems = pageWithNulls.Items.Where(i => i != null).Select(i => i!);
            return new Page<TO> {
                Items = cleanedItems,
                MaxPage = pageWithNulls.MaxPage,
                ItemCount = pageWithNulls.ItemCount,
                PageNumber = pageWithNulls.PageNumber
            };
        }
    }
}
