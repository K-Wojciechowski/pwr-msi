#nullable enable
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pwr_msi.Models;

namespace pwr_msi.Services {
    public class AuthMiddleware {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context, MsiDbContext dbContext, ILogger<AuthMiddleware> logger) {
            var authResult = await context.AuthenticateAsync();
            User? user = null;
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
            if (endpoint != null) {
                var allowedForEndpoint = await AllowedForEndpoint(dbContext, user, context, endpoint, logger);
                if (!allowedForEndpoint) {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            await _next(context);
        }

        private async Task<bool> AllowedForEndpoint(MsiDbContext dbContext, User? user, HttpContext httpContext,
            Endpoint endpoint, ILogger<AuthMiddleware> logger) {
            var adminMeta = endpoint.Metadata.GetMetadata<AdminAuthorizeAttribute>();
            var restaurantMeta = endpoint.Metadata.GetMetadata<RestaurantAuthorizeAttribute>();
            if (adminMeta != null && user?.IsAdmin != true) {
                return false;
            }

            if (restaurantMeta == null) {
                return true;
            }

            if (user == null) {
                return false;
            }

            var restaurantIdStr = (string?) httpContext.GetRouteData().Values[restaurantMeta.RouteArgument];
            if (!int.TryParse(restaurantIdStr, out var restaurantId)) {
                logger.LogCritical("Unable to parse restaurant ID from input: {RestaurantIdStr}", restaurantIdStr);
                return false;
            }

            var restaurantUser = await dbContext.RestaurantUsers
                .Where(ru => ru.UserId == user.UserId && ru.RestaurantId == restaurantId).FirstOrDefaultAsync();

            if (restaurantUser == null) {
                return false;
            }

            return restaurantMeta.Permission switch {
                RestaurantPermission.MANAGE => restaurantUser.CanManage,
                RestaurantPermission.ACCEPT => restaurantUser.CanAcceptOrders,
                RestaurantPermission.DELIVER => restaurantUser.CanDeliverOrders,
                _ => false,
            };
        }
    }
}
