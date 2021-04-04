using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using pwr_msi.AuthPolicies;
using pwr_msi.Controllers;
using pwr_msi.Models;

namespace pwr_msi.Services {
    public class AuthMiddleware {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context, MsiDbContext dbContext) {
            var authResult = await context.AuthenticateAsync();
            User user = null;
            if (authResult.Succeeded) {
                var userIdStr = authResult.Principal?.FindFirst(match: c => c.Type == AuthService.ClaimUserId)?.Value;
                var userId = Utils.TryParseInt(userIdStr);
                if (userId.HasValue) {
                    user = await dbContext.Users.FindAsync(userId);
                    context.Items[key: "User"] = user;
                    context.Items[key: "UserID"] = userId;
                }
            }
            var endpoint = context.GetEndpoint();
            var adminMeta = endpoint?.Metadata.GetMetadata<AdminAuthorizeAttribute>();
            if (adminMeta != null && user?.IsAdmin != true) {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            await _next(context);
        }
    }
}
