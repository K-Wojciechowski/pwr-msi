using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;
using pwr_msi.Models.Dto;

namespace pwr_msi {
    public static class Utils {
        public static int? TryParseInt(string s) {
            int i;
            var res = int.TryParse(s, out i);
            return res ? i : null;
        }

        public static User UserFromContext(HttpContext context) {
            return (User) context.Items[key: "User"];
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
    }
}
