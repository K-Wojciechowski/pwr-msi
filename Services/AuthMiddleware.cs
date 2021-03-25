using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace pwr_msi.Services {
    public class AuthMiddleware {
        private readonly RequestDelegate _next;
        // private readonly MsiDbContext _dbContext;

        public AuthMiddleware(RequestDelegate next) {
            _next = next;
            // _dbContext = dbContext;
        }

        public async Task Invoke(HttpContext context, MsiDbContext dbContext) {
            var authResult = await context.AuthenticateAsync();
            if (authResult.Succeeded) {
                var userIdStr = authResult.Principal?.FindFirst(c => c.Type == AuthService.ClaimUserId)?.Value;
                var userId = Utils.TryParseInt(userIdStr);
                if (userId.HasValue) {
                    var user = await dbContext.Users.FindAsync(userId);
                    context.Items["User"] = user;
                }
            }

            await _next(context);
        }
    }
}
