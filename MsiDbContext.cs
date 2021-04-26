using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;

namespace pwr_msi {
    public class MsiDbContext : DbContext {
        public MsiDbContext(DbContextOptions<MsiDbContext> options) : base(options) {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<BalanceRepayment> BalanceRepayments { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemOptionItem> MenuItemOptionItems { get; set; }
        public DbSet<MenuItemOptionList> MenuItemOptionLists { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemCustomization> OrderItemCustomizations { get; set; }
        public DbSet<OrderTask> OrderTasks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<RestaurantUser> RestaurantUsers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VerificationToken> VerificationTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().Property(u => u.IsActive).HasDefaultValue(true);

            modelBuilder.Entity<User>().HasMany(navigationExpression: u => u.Addresses)
                .WithMany(navigationExpression: a => a.Users);
            modelBuilder.Entity<User>().HasOne(navigationExpression: u => u.BillingAddress);
            modelBuilder.Entity<User>().HasIndex(indexExpression: u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(indexExpression: u => u.Email).IsUnique();
            modelBuilder.Entity<Restaurant>().HasMany(navigationExpression: r => r.Users)
                .WithMany(navigationExpression: u => u.Restaurants)
                .UsingEntity<RestaurantUser>(
                    configureRight: j =>
                        j.HasOne(navigationExpression: ru => ru.User)
                            .WithMany(navigationExpression: u => u.RestaurantUsers)
                            .HasForeignKey(foreignKeyExpression: ru => ru.UserId),
                    configureLeft: j => j.HasOne(navigationExpression: ru => ru.Restaurant)
                        .WithMany(navigationExpression: r => r.RestaurantUsers)
                        .HasForeignKey(foreignKeyExpression: ru => ru.RestaurantId),
                    configureJoinEntityType: j => j.HasKey(keyExpression: ru => new {ru.RestaurantId, ru.UserId})
                );
        }
    }
}
