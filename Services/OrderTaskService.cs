#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using pwr_msi.Models;
using pwr_msi.Models.Enum;

namespace pwr_msi.Services {
    public class OrderTaskService {
        private MsiDbContext _dbContext;

        public OrderTaskService(MsiDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<bool> TryCompleteTask(Order order, OrderTaskType task, User? completedBy) {
            var canPerform = OrderTaskTypeSettings.allowedTransitions[order.LastTaskType].Contains(task);
            var userAllowed = await IsUserAllowedToPerform(task, order, completedBy);
            if (!canPerform || !userAllowed) return false;
            var orderTask = await _dbContext.OrderTasks.Where(t => t.OrderId == order.OrderId && t.Task == task)
                .FirstOrDefaultAsync();
            if (orderTask != null) {
                orderTask.DateCompleted = Utils.Now();
                orderTask.CompletedBy = completedBy;
            } else {
                orderTask = new OrderTask {
                    Order = order, CompletedBy = completedBy, DateCompleted = Utils.Now(),
                };
                await _dbContext.AddAsync(orderTask);
            }

            var oldStatus = order.Status;
            order.Status = OrderTaskTypeSettings.statusByTaskType[task];
            await _dbContext.SaveChangesAsync();
            await ReactToTaskCompletion(order, oldStatus, order.Status);

            return true;
        }

        private async Task ReactToTaskCompletion(Order order, OrderStatus oldStatus, OrderStatus newStatus) {
            if (newStatus == OrderStatus.DELIVERED) {
                await TryCompleteTask(order, OrderTaskType.COMPLETE, completedBy: null);
            }
        }

        private async Task<bool> IsUserAllowedToPerform(OrderTaskType task, Order order, User? user) {
            var allowedPerformers = OrderTaskTypeSettings.allowedPerformers[task];
            var userId = user?.UserId;
            var grantTasks = allowedPerformers.Select(performer =>
                performer switch {
                    OrderTaskPerformer.CUSTOMER => Task.FromResult(order.Customer == user),
                    OrderTaskPerformer.ACCEPTOR =>
                        _dbContext.RestaurantUsers.Where(ru =>
                                ru.UserId == userId && ru.RestaurantId == order.RestaurantId && ru.CanAcceptOrders)
                            .AnyAsync(),

                    OrderTaskPerformer.DELIVERY =>
                        _dbContext.RestaurantUsers.Where(ru =>
                                ru.UserId == userId && ru.RestaurantId == order.RestaurantId && ru.CanDeliverOrders)
                            .AnyAsync(),
                    OrderTaskPerformer.SYSTEM => Task.FromResult(user == null),
                    _ => throw new ArgumentOutOfRangeException(nameof(performer), performer,
                        message: "unknown performer"),
                });
            return (await Task.WhenAll(grantTasks)).Any(grant => grant);
        }

        public async Task TryMarkAsPaid(int orderId) {
            var order = await _dbContext.Orders.FindAsync(orderId);
            var completedPayments = _dbContext.Payments.Where(p =>
                p.OrderId == orderId && !p.IsReturn && p.Status == PaymentStatus.COMPLETED);
            var alreadyPaid = await completedPayments.SumAsync(p => p.Amount);
            if (alreadyPaid >= order.TotalPrice) {
                await TryCompleteTask(order, OrderTaskType.PAY, order.Customer);
            }
        }
    }
}
