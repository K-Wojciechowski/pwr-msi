using Microsoft.EntityFrameworkCore;
using pwr_msi.Models;

namespace pwr_msi {
    public class MsiDbContext : DbContext {
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

        public MsiDbContext(DbContextOptions<MsiDbContext> options): base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().HasMany(u => u.Addresses).WithMany(a => a.Users);
            modelBuilder.Entity<User>().HasOne(u => u.BillingAddress);
            modelBuilder.Entity<Restaurant>().HasMany(r => r.Users).WithMany(u => u.Restaurants)
                .UsingEntity<RestaurantUser>(
                    j => j.HasOne(ru => ru.User).WithMany(u => u.RestaurantUsers).HasForeignKey(ru => ru.UserId),
                    j => j.HasOne(ru => ru.Restaurant).WithMany(r => r.RestaurantUsers)
                        .HasForeignKey(ru => ru.RestaurantId),
                    j => j.HasKey(ru => new {ru.RestaurantId, ru.UserId})
                );
        }
    }
}
