using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using pwr_msi;

namespace pwr_msi.Migrations
{
    [DbContext(typeof(MsiDbContext))]
    [Migration("20210526212437_AddressesAndMenuExtensions")]
    partial class AddressesAndMenuExtensions
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("AddressUser", b =>
                {
                    b.Property<int>("AddressesAddressId")
                        .HasColumnType("integer");

                    b.Property<int>("UsersUserId")
                        .HasColumnType("integer");

                    b.HasKey("AddressesAddressId", "UsersUserId");

                    b.HasIndex("UsersUserId");

                    b.ToTable("AddressUser");
                });

            modelBuilder.Entity("CuisineRestaurant", b =>
                {
                    b.Property<int>("CuisinesCuisineId")
                        .HasColumnType("integer");

                    b.Property<int>("RestaurantsRestaurantId")
                        .HasColumnType("integer");

                    b.HasKey("CuisinesCuisineId", "RestaurantsRestaurantId");

                    b.HasIndex("RestaurantsRestaurantId");

                    b.ToTable("CuisineRestaurant");
                });

            modelBuilder.Entity("pwr_msi.Models.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Addressee")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<string>("HouseNumber")
                        .HasColumnType("text");

                    b.Property<float>("Latitude")
                        .HasColumnType("real");

                    b.Property<float>("Longitude")
                        .HasColumnType("real");

                    b.Property<string>("PostCode")
                        .HasColumnType("text");

                    b.Property<string>("Street")
                        .HasColumnType("text");

                    b.HasKey("AddressId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("pwr_msi.Models.Cuisine", b =>
                {
                    b.Property<int>("CuisineId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("CuisineId");

                    b.ToTable("Cuisines");
                });

            modelBuilder.Entity("pwr_msi.Models.MenuCategory", b =>
                {
                    b.Property<int>("MenuCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("MenuCategoryOrder")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("RestaurantId")
                        .HasColumnType("integer");

                    b.Property<ZonedDateTime>("ValidFrom")
                        .HasColumnType("timestamp with time zone");

                    b.Property<ZonedDateTime?>("ValidUntil")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("MenuCategoryId");

                    b.HasIndex("RestaurantId");

                    b.ToTable("MenuCategories");
                });

            modelBuilder.Entity("pwr_msi.Models.MenuItem", b =>
                {
                    b.Property<int>("MenuItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<int>("AmountUnit")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<int>("MenuCategoryId")
                        .HasColumnType("integer");

                    b.Property<int>("MenuOrder")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("RestaurantId")
                        .HasColumnType("integer");

                    b.Property<ZonedDateTime>("ValidFrom")
                        .HasColumnType("timestamp with time zone");

                    b.Property<ZonedDateTime?>("ValidUntil")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("MenuItemId");

                    b.HasIndex("MenuCategoryId");

                    b.HasIndex("RestaurantId");

                    b.ToTable("MenuItems");
                });

            modelBuilder.Entity("pwr_msi.Models.MenuItemOptionItem", b =>
                {
                    b.Property<int>("MenuItemOptionItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("MenuItemOptionItemOrder")
                        .HasColumnType("integer");

                    b.Property<int>("MenuItemOptionListId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.HasKey("MenuItemOptionItemId");

                    b.HasIndex("MenuItemOptionListId");

                    b.ToTable("MenuItemOptionItems");
                });

            modelBuilder.Entity("pwr_msi.Models.MenuItemOptionList", b =>
                {
                    b.Property<int>("MenuItemOptionListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("IsMultipleChoice")
                        .HasColumnType("boolean");

                    b.Property<int>("MenuItemId")
                        .HasColumnType("integer");

                    b.Property<int>("MenuItemOptionListOrder")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("MenuItemOptionListId");

                    b.HasIndex("MenuItemId");

                    b.ToTable("MenuItemOptionLists");
                });

            modelBuilder.Entity("pwr_msi.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AddressId")
                        .HasColumnType("integer");

                    b.Property<ZonedDateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<ZonedDateTime?>("Delivered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DeliveryNotes")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("DeliveryPersonId")
                        .HasColumnType("integer");

                    b.Property<int>("RestaurantId")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("numeric");

                    b.Property<ZonedDateTime>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("OrderId");

                    b.HasIndex("AddressId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("DeliveryPersonId");

                    b.HasIndex("RestaurantId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("pwr_msi.Models.OrderItem", b =>
                {
                    b.Property<int>("OrderItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<int>("MenuItemId")
                        .HasColumnType("integer");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("numeric");

                    b.HasKey("OrderItemId");

                    b.HasIndex("MenuItemId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("pwr_msi.Models.OrderItemCustomization", b =>
                {
                    b.Property<int>("OrderItemCustomizationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("MenuItemOptionItemId")
                        .HasColumnType("integer");

                    b.Property<int>("OrderItemId")
                        .HasColumnType("integer");

                    b.HasKey("OrderItemCustomizationId");

                    b.HasIndex("MenuItemOptionItemId");

                    b.HasIndex("OrderItemId");

                    b.ToTable("OrderItemCustomizations");
                });

            modelBuilder.Entity("pwr_msi.Models.OrderTask", b =>
                {
                    b.Property<int>("OrderTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AssigneeRestaurantId")
                        .HasColumnType("integer");

                    b.Property<int?>("AssigneeType")
                        .HasColumnType("integer");

                    b.Property<int?>("AssigneeUserId")
                        .HasColumnType("integer");

                    b.Property<int?>("CompletedById")
                        .HasColumnType("integer");

                    b.Property<ZonedDateTime?>("DateCompleted")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<int>("Task")
                        .HasColumnType("integer");

                    b.HasKey("OrderTaskId");

                    b.HasIndex("AssigneeRestaurantId");

                    b.HasIndex("AssigneeUserId");

                    b.HasIndex("CompletedById");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderTasks");
                });

            modelBuilder.Entity("pwr_msi.Models.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<ZonedDateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text");

                    b.Property<string>("ExternalPaymentId")
                        .HasColumnType("text");

                    b.Property<bool>("IsBalanceRepayment")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsReturn")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsTargettingBalance")
                        .HasColumnType("boolean");

                    b.Property<int?>("OrderId")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<ZonedDateTime>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("PaymentId");

                    b.HasIndex("OrderId");

                    b.HasIndex("UserId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("pwr_msi.Models.Restaurant", b =>
                {
                    b.Property<int>("RestaurantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AddressId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Logo")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Website")
                        .HasColumnType("text");

                    b.HasKey("RestaurantId");

                    b.HasIndex("AddressId");

                    b.ToTable("Restaurants");
                });

            modelBuilder.Entity("pwr_msi.Models.RestaurantUser", b =>
                {
                    b.Property<int>("RestaurantId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<bool>("CanAcceptOrders")
                        .HasColumnType("boolean");

                    b.Property<bool>("CanDeliverOrders")
                        .HasColumnType("boolean");

                    b.Property<bool>("CanManage")
                        .HasColumnType("boolean");

                    b.HasKey("RestaurantId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("RestaurantUsers");
                });

            modelBuilder.Entity("pwr_msi.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric");

                    b.Property<int?>("BillingAddressId")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.HasIndex("BillingAddressId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("pwr_msi.Models.VerificationToken", b =>
                {
                    b.Property<int>("VerificationTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("IsUsed")
                        .HasColumnType("boolean");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<int>("TokenType")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<Instant>("ValidUntil")
                        .HasColumnType("timestamp");

                    b.HasKey("VerificationTokenId");

                    b.HasIndex("UserId");

                    b.ToTable("VerificationTokens");
                });

            modelBuilder.Entity("AddressUser", b =>
                {
                    b.HasOne("pwr_msi.Models.Address", null)
                        .WithMany()
                        .HasForeignKey("AddressesAddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwr_msi.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CuisineRestaurant", b =>
                {
                    b.HasOne("pwr_msi.Models.Cuisine", null)
                        .WithMany()
                        .HasForeignKey("CuisinesCuisineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwr_msi.Models.Restaurant", null)
                        .WithMany()
                        .HasForeignKey("RestaurantsRestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("pwr_msi.Models.MenuCategory", b =>
                {
                    b.HasOne("pwr_msi.Models.Restaurant", "Restaurant")
                        .WithMany("MenuCategories")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("pwr_msi.Models.MenuItem", b =>
                {
                    b.HasOne("pwr_msi.Models.MenuCategory", "MenuCategory")
                        .WithMany("Items")
                        .HasForeignKey("MenuCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwr_msi.Models.Restaurant", "Restaurant")
                        .WithMany("MenuItems")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuCategory");

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("pwr_msi.Models.MenuItemOptionItem", b =>
                {
                    b.HasOne("pwr_msi.Models.MenuItemOptionList", "MenuItemOptionList")
                        .WithMany("Items")
                        .HasForeignKey("MenuItemOptionListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuItemOptionList");
                });

            modelBuilder.Entity("pwr_msi.Models.MenuItemOptionList", b =>
                {
                    b.HasOne("pwr_msi.Models.MenuItem", "MenuItem")
                        .WithMany("Options")
                        .HasForeignKey("MenuItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuItem");
                });

            modelBuilder.Entity("pwr_msi.Models.Order", b =>
                {
                    b.HasOne("pwr_msi.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwr_msi.Models.User", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwr_msi.Models.User", "DeliveryPerson")
                        .WithMany()
                        .HasForeignKey("DeliveryPersonId");

                    b.HasOne("pwr_msi.Models.Restaurant", "Restaurant")
                        .WithMany()
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("Customer");

                    b.Navigation("DeliveryPerson");

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("pwr_msi.Models.OrderItem", b =>
                {
                    b.HasOne("pwr_msi.Models.MenuItem", "MenuItem")
                        .WithMany()
                        .HasForeignKey("MenuItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwr_msi.Models.Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuItem");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("pwr_msi.Models.OrderItemCustomization", b =>
                {
                    b.HasOne("pwr_msi.Models.MenuItemOptionItem", "MenuItemOptionItem")
                        .WithMany()
                        .HasForeignKey("MenuItemOptionItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwr_msi.Models.OrderItem", "OrderItem")
                        .WithMany("Customizations")
                        .HasForeignKey("OrderItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuItemOptionItem");

                    b.Navigation("OrderItem");
                });

            modelBuilder.Entity("pwr_msi.Models.OrderTask", b =>
                {
                    b.HasOne("pwr_msi.Models.Restaurant", "AssigneeRestaurant")
                        .WithMany()
                        .HasForeignKey("AssigneeRestaurantId");

                    b.HasOne("pwr_msi.Models.User", "AssigneeUser")
                        .WithMany()
                        .HasForeignKey("AssigneeUserId");

                    b.HasOne("pwr_msi.Models.User", "CompletedBy")
                        .WithMany()
                        .HasForeignKey("CompletedById");

                    b.HasOne("pwr_msi.Models.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssigneeRestaurant");

                    b.Navigation("AssigneeUser");

                    b.Navigation("CompletedBy");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("pwr_msi.Models.Payment", b =>
                {
                    b.HasOne("pwr_msi.Models.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId");

                    b.HasOne("pwr_msi.Models.User", "User")
                        .WithMany("Payments")
                        .HasForeignKey("UserId");

                    b.Navigation("Order");

                    b.Navigation("User");
                });

            modelBuilder.Entity("pwr_msi.Models.Restaurant", b =>
                {
                    b.HasOne("pwr_msi.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");
                });

            modelBuilder.Entity("pwr_msi.Models.RestaurantUser", b =>
                {
                    b.HasOne("pwr_msi.Models.Restaurant", "Restaurant")
                        .WithMany("RestaurantUsers")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwr_msi.Models.User", "User")
                        .WithMany("RestaurantUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Restaurant");

                    b.Navigation("User");
                });

            modelBuilder.Entity("pwr_msi.Models.User", b =>
                {
                    b.HasOne("pwr_msi.Models.Address", "BillingAddress")
                        .WithMany()
                        .HasForeignKey("BillingAddressId");

                    b.Navigation("BillingAddress");
                });

            modelBuilder.Entity("pwr_msi.Models.VerificationToken", b =>
                {
                    b.HasOne("pwr_msi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("pwr_msi.Models.MenuCategory", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("pwr_msi.Models.MenuItem", b =>
                {
                    b.Navigation("Options");
                });

            modelBuilder.Entity("pwr_msi.Models.MenuItemOptionList", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("pwr_msi.Models.Order", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("pwr_msi.Models.OrderItem", b =>
                {
                    b.Navigation("Customizations");
                });

            modelBuilder.Entity("pwr_msi.Models.Restaurant", b =>
                {
                    b.Navigation("MenuCategories");

                    b.Navigation("MenuItems");

                    b.Navigation("RestaurantUsers");
                });

            modelBuilder.Entity("pwr_msi.Models.User", b =>
                {
                    b.Navigation("Payments");

                    b.Navigation("RestaurantUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
