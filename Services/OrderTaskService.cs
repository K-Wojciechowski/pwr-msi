#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using pwr_msi.Models;
using pwr_msi.Models.Enum;

namespace pwr_msi.Services {
    public class OrderTaskService {
        private readonly MsiDbContext _dbContext;
        private readonly IServiceProvider _services;

        public OrderTaskService(MsiDbContext dbContext, IServiceProvider services) {
            _dbContext = dbContext;
            _services = services;
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
                orderTask = new OrderTask {Order = order, CompletedBy = completedBy, DateCompleted = Utils.Now(),};
                await _dbContext.AddAsync(orderTask);
            }

            var oldStatus = order.Status;
            order.Status = OrderTaskTypeSettings.statusByTaskType[task];
            order.Updated = Utils.Now();
            await _dbContext.SaveChangesAsync();
            await ReactToTaskCompletion(order, oldStatus, order.Status);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task<bool> TryRegisterTask(Order order, OrderTaskType task, AssigneeType? assigneeType = null,
            int? assigneeUserId = null,
            int? assigneeRestaurantId = null) {
            var canPerform = OrderTaskTypeSettings.allowedTransitions[order.LastTaskType].Contains(task);
            if (!canPerform) return false;
            var orderTask = new OrderTask {
                Order = order,
                AssigneeType = assigneeType,
                AssigneeRestaurantId = assigneeRestaurantId,
                AssigneeUserId = assigneeUserId,
            };
            await _dbContext.AddAsync(orderTask);
            order.Updated = Utils.Now();
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task ReactToTaskCompletion(Order order, OrderStatus oldStatus, OrderStatus newStatus) {
            switch (newStatus) {
                case OrderStatus.PAID:
                    await TryRegisterTask(order, OrderTaskType.DECIDE, AssigneeType.RESTAURANT,
                        assigneeRestaurantId: order.RestaurantId);
                    break;
                case OrderStatus.DELIVERED:
                    await TryCompleteTask(order, OrderTaskType.COMPLETE, completedBy: null);
                    break;
                case OrderStatus.CANCELLED:
                case OrderStatus.REJECTED:
                    // This works around a dependency cycle.
                    var task = _services.GetService<PaymentService>()?.ReturnOrderPayment(order);
                    if (task != null) {
                        await task;
                        order.Updated = Utils.Now();
                    }
                    break;
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
            var completed = false;
            if (alreadyPaid >= order.TotalPrice) {
                completed = await TryCompleteTask(order, OrderTaskType.PAY, order.Customer);
            }
        }
    }
}
