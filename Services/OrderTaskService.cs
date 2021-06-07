#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using pwr_msi.Models;
using pwr_msi.Models.Enum;

namespace pwr_msi.Services {
    public class OrderTaskService {
        private readonly MsiDbContext _dbContext;
        private readonly IServiceProvider _services;
        private readonly ILogger<OrderTaskService> _logger;

        public OrderTaskService(MsiDbContext dbContext, IServiceProvider services, ILogger<OrderTaskService> logger) {
            _dbContext = dbContext;
            _services = services;
            _logger = logger;
        }

        public async Task<bool> TryCompleteTask(Order order, OrderTaskType task, User? completedBy) {
            var canPerform = OrderTaskTypeSettings.allowedTransitions[order.LastTaskType].Contains(task);
            var userAllowed = await IsUserAllowedToPerform(task, order, completedBy);
            if (!canPerform || !userAllowed) {
                _logger.LogWarning(
                    "User {UserId} tried to complete task {Task} in order {OrderId}, but this operation is not allowed (canPerform = {CanPerform}, userAllowed = {UserAllowed})",
                    completedBy?.UserId, task, order.OrderId, canPerform, userAllowed);
                return false;
            }

            var orderTask = await _dbContext.OrderTasks.Where(t => t.OrderId == order.OrderId && t.Task == task)
                .FirstOrDefaultAsync();
            if (orderTask != null) {
                orderTask.DateCompleted = Utils.Now();
                orderTask.CompletedBy = completedBy;
            } else {
                orderTask = new OrderTask {Order = order, CompletedBy = completedBy, DateCompleted = Utils.Now(),};
                await _dbContext.AddAsync(orderTask);
            }

            _logger.LogInformation("Performing {Task} in order {OrderId} (User {UserId})", task, order.OrderId,
                completedBy?.UserId);

            var oldStatus = order.Status;
            order.Status = OrderTaskTypeSettings.statusByTaskType[task];
            order.Updated = Utils.Now();
            await _dbContext.SaveChangesAsync();
            await ReactToTaskCompletion(order, oldStatus, order.Status);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TryRegisterTask(Order order, OrderTaskType task, AssigneeType? assigneeType = null,
            int? assigneeRestaurantId = null, int? assigneeUserId = null) {
            var canPerform = OrderTaskTypeSettings.allowedTransitions[order.LastTaskType].Contains(task);
            if (!canPerform) {
                _logger.LogWarning(
                    "Tried to register task {Task} in order {OrderId}, but this operation is not allowed", task,
                    order.OrderId);
                return false;
            }

            var orderTask = new OrderTask {
                Order = order,
                Task = task,
                AssigneeType = assigneeType,
                AssigneeRestaurantId = assigneeRestaurantId,
                AssigneeUserId = assigneeUserId,
            };
            await _dbContext.AddAsync(orderTask);
            _logger.LogInformation("Registering {Task} in order {OrderId} (assignee: {AssigneeType} R:{ArId} U:{AuId})",
                task,
                order.OrderId, assigneeType, assigneeRestaurantId, assigneeUserId);
            order.Updated = Utils.Now();
            await _dbContext.SaveChangesAsync();

            if (orderTask.AssigneeType != null) {
                await ReactToTaskAssignment(order, orderTask);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> TryRegisterOrAssignTask(Order order, OrderTaskType task, AssigneeType assigneeType,
            int? assigneeRestaurantId = null, int? assigneeUserId = null) {
            var canPerform = OrderTaskTypeSettings.allowedTransitions[order.LastTaskType].Contains(task);
            if (!canPerform) return false;
            var existingTask = await _dbContext.OrderTasks.Where(ot => ot.OrderId == order.OrderId && ot.Task == task)
                .FirstOrDefaultAsync();
            if (existingTask == null) {
                return await TryRegisterTask(order, task, assigneeType, assigneeRestaurantId: assigneeRestaurantId,
                    assigneeUserId: assigneeUserId);
            }

            existingTask.AssigneeType = assigneeType;
            existingTask.AssigneeRestaurantId = assigneeRestaurantId;
            existingTask.AssigneeUserId = assigneeUserId;
            _logger.LogInformation("Reassigning {Task} in order {OrderId} to {AssigneeType} R:{ArId} U:{AuId}", task,
                order.OrderId, assigneeType, assigneeRestaurantId, assigneeUserId);
            order.Updated = Utils.Now();
            await _dbContext.SaveChangesAsync();

            if (existingTask.AssigneeType != null) {
                await ReactToTaskAssignment(order, existingTask);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }

        private async Task ReactToTaskCompletion(Order order, OrderStatus oldStatus, OrderStatus newStatus) {
            if (oldStatus == newStatus) return;
            switch (newStatus) {
                case OrderStatus.PAID:
                    _logger.LogInformation("Reaction: Registering decision task in order {OrderId}", order.OrderId);
                    await TryRegisterTask(order, OrderTaskType.DECIDE, AssigneeType.RESTAURANT,
                        assigneeRestaurantId: order.RestaurantId);
                    break;
                case OrderStatus.PREPARED:
                    _logger.LogInformation("Reaction: Registering delivery task in order {OrderId}", order.OrderId);
                    await TryRegisterTask(order, OrderTaskType.DELIVER, AssigneeType.RESTAURANT,
                        assigneeRestaurantId: order.RestaurantId);
                    break;
                case OrderStatus.DELIVERED:
                    _logger.LogInformation("Reaction: Completing order {OrderId} after delivery", order.OrderId);
                    order.Delivered = Utils.Now();
                    await _dbContext.SaveChangesAsync();
                    await TryCompleteTask(order, OrderTaskType.COMPLETE, completedBy: null);
                    break;
                case OrderStatus.CANCELLED:
                case OrderStatus.REJECTED:
                    _logger.LogInformation("Reaction: Returning payment for order {OrderId} due to {Status}",
                        order.OrderId, newStatus);
                    // This works around a dependency cycle.
                    var task = _services.GetService<PaymentService>()?.ReturnOrderPayment(order);
                    if (task != null) {
                        await task;
                        order.Updated = Utils.Now();
                    }

                    break;
            }
        }

        private async Task ReactToTaskAssignment(Order order, OrderTask orderTask) {
            if (orderTask.Task == OrderTaskType.DELIVER && orderTask.AssigneeType != AssigneeType.USER) {
                    _logger.LogInformation("Reaction: Setting delivery person of order {OrderId} to {DeliveryId}",
                        order.OrderId, orderTask.AssigneeUserId);
                    order.DeliveryPersonId = orderTask.AssigneeUserId;
                    await _dbContext.SaveChangesAsync();
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
