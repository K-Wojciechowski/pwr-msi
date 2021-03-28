using Microsoft.AspNetCore.Http;
using pwr_msi.Models;

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
    }
}
