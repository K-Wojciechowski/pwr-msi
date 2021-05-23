using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;
using pwr_msi.Models.Dto;

namespace pwr_msi.Services {
    public class AdminCommonService {
        private readonly MsiDbContext _dbContext;

        public AdminCommonService(MsiDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task UpdateRestaurantUsers(IEnumerable<RestaurantUserDto> incomingRestaurantUsers,
            Func<RestaurantUser, bool> criteria) {
            var dbRestaurantUsers = _dbContext.RestaurantUsers
                .Include(ru => ru.Restaurant)
                .Include(ru => ru.User)
                .Where(criteria);
            var incomingDict = incomingRestaurantUsers.ToDictionary(rud =>
                new RestaurantUserKey(rud.User.UserId, rud.Restaurant.RestaurantId));
            var dbDict = dbRestaurantUsers.ToDictionary(ru =>
                new RestaurantUserKey(ru.UserId, ru.RestaurantId));

            var incomingKeys = incomingDict.Keys.ToHashSet();
            var dbKeys = dbDict.Keys.ToHashSet();

            var added = SetDiff(incomingKeys, dbKeys);
            var deleted = SetDiff(dbKeys, incomingKeys);
            var updated = new HashSet<RestaurantUserKey>(incomingKeys);
            updated.IntersectWith(dbKeys);

            foreach (var key in added) {
                var rud = incomingDict[key];
                var ru = new RestaurantUser {
                    RestaurantId = rud.Restaurant.RestaurantId,
                    UserId = rud.User.UserId,
                    CanManage = rud.CanManage,
                    CanAcceptOrders = rud.CanAcceptOrders,
                    CanDeliverOrders = rud.CanDeliverOrders,
                };
                await _dbContext.AddAsync(ru);
            }

            foreach (var key in updated) {
                var rud = incomingDict[key];
                var ru = dbDict[key];
                ru.CanManage = rud.CanManage;
                ru.CanAcceptOrders = rud.CanAcceptOrders;
                ru.CanDeliverOrders = rud.CanDeliverOrders;
            }

            _dbContext.RemoveRange(deleted.Select(key => dbDict[key]));

            await _dbContext.SaveChangesAsync();
        }

        private static HashSet<T> SetDiff<T>(HashSet<T> first, HashSet<T> second) {
            var output = new HashSet<T>(first);
            output.ExceptWith(second);
            return output;
        }
    }

    internal record RestaurantUserKey(int UserId, int RestaurantId);
}
