using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace pwr_msi.Migrations {
    public partial class InitialCreate : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new {
                    AddressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Addressee = table.Column<string>(type: "text", nullable: true),
                    FirstLine = table.Column<string>(type: "text", nullable: true),
                    SecondLine = table.Column<string>(type: "text", nullable: true),
                    PostCode = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table => { table.PrimaryKey(name: "PK_Addresses", columns: x => x.AddressId); });

            migrationBuilder.CreateTable(
                name: "Cuisines",
                columns: table => new {
                    CuisineId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table => { table.PrimaryKey(name: "PK_Cuisines", columns: x => x.CuisineId); });

            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new {
                    RestaurantId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AddressId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_Restaurants", columns: x => x.RestaurantId);
                    table.ForeignKey(
                        name: "FK_Restaurants_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    BillingAddressId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_Users", columns: x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Addresses_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuisineRestaurant",
                columns: table => new {
                    CuisinesCuisineId = table.Column<int>(type: "integer", nullable: false),
                    RestaurantsRestaurantId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_CuisineRestaurant",
                        columns: x => new {x.CuisinesCuisineId, x.RestaurantsRestaurantId});
                    table.ForeignKey(
                        name: "FK_CuisineRestaurant_Cuisines_CuisinesCuisineId",
                        column: x => x.CuisinesCuisineId,
                        principalTable: "Cuisines",
                        principalColumn: "CuisineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuisineRestaurant_Restaurants_RestaurantsRestaurantId",
                        column: x => x.RestaurantsRestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuCategories",
                columns: table => new {
                    MenuCategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RestaurantId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_MenuCategories", columns: x => x.MenuCategoryId);
                    table.ForeignKey(
                        name: "FK_MenuCategories_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressUser",
                columns: table => new {
                    AddressesAddressId = table.Column<int>(type: "integer", nullable: false),
                    UsersUserId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_AddressUser", columns: x => new {x.AddressesAddressId, x.UsersUserId});
                    table.ForeignKey(
                        name: "FK_AddressUser_Addresses_AddressesAddressId",
                        column: x => x.AddressesAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressUser_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemTasks",
                columns: table => new {
                    OrderItemTaskId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Task = table.Column<int>(type: "integer", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CompletedById = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_OrderItemTasks", columns: x => x.OrderItemTaskId);
                    table.ForeignKey(
                        name: "FK_OrderItemTasks_Users_CompletedById",
                        column: x => x.CompletedById,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new {
                    OrderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RestaurantId = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    DeliveryPersonId = table.Column<int>(type: "integer", nullable: true),
                    AddressId = table.Column<int>(type: "integer", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    DeliveryNotes = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_Orders", columns: x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Users_DeliveryPersonId",
                        column: x => x.DeliveryPersonId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantUsers",
                columns: table => new {
                    RestaurantId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CanManage = table.Column<bool>(type: "boolean", nullable: false),
                    CanAcceptOrders = table.Column<bool>(type: "boolean", nullable: false),
                    CanDeliverOrders = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_RestaurantUsers", columns: x => new {x.RestaurantId, x.UserId});
                    table.ForeignKey(
                        name: "FK_RestaurantUsers_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestaurantUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new {
                    MenuItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountUnit = table.Column<int>(type: "integer", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MenuOrder = table.Column<int>(type: "integer", nullable: false),
                    MenuCategoryId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_MenuItems", columns: x => x.MenuItemId);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuCategories_MenuCategoryId",
                        column: x => x.MenuCategoryId,
                        principalTable: "MenuCategories",
                        principalColumn: "MenuCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemOptionLists",
                columns: table => new {
                    MenuItemOptionListId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IsMultipleChoice = table.Column<bool>(type: "boolean", nullable: false),
                    MenuItemId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_MenuItemOptionLists", columns: x => x.MenuItemOptionListId);
                    table.ForeignKey(
                        name: "FK_MenuItemOptionLists_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "MenuItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new {
                    OrderItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    MenuItemId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_OrderItems", columns: x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "MenuItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemOptionItems",
                columns: table => new {
                    MenuItemOptionItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    MenuItemOptionListId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_MenuItemOptionItems", columns: x => x.MenuItemOptionItemId);
                    table.ForeignKey(
                        name: "FK_MenuItemOptionItems_MenuItemOptionLists_MenuItemOptionListId",
                        column: x => x.MenuItemOptionListId,
                        principalTable: "MenuItemOptionLists",
                        principalColumn: "MenuItemOptionListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemCustomizations",
                columns: table => new {
                    OrderItemCustomizationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderItemId = table.Column<int>(type: "integer", nullable: false),
                    MenuItemOptionItemId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_OrderItemCustomizations", columns: x => x.OrderItemCustomizationId);
                    table.ForeignKey(
                        name: "FK_OrderItemCustomizations_MenuItemOptionItems_MenuItemOptionI~",
                        column: x => x.MenuItemOptionItemId,
                        principalTable: "MenuItemOptionItems",
                        principalColumn: "MenuItemOptionItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemCustomizations_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressUser_UsersUserId",
                table: "AddressUser",
                column: "UsersUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CuisineRestaurant_RestaurantsRestaurantId",
                table: "CuisineRestaurant",
                column: "RestaurantsRestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_RestaurantId",
                table: "MenuCategories",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemOptionItems_MenuItemOptionListId",
                table: "MenuItemOptionItems",
                column: "MenuItemOptionListId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemOptionLists_MenuItemId",
                table: "MenuItemOptionLists",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_MenuCategoryId",
                table: "MenuItems",
                column: "MenuCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemCustomizations_MenuItemOptionItemId",
                table: "OrderItemCustomizations",
                column: "MenuItemOptionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemCustomizations_OrderItemId",
                table: "OrderItemCustomizations",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MenuItemId",
                table: "OrderItems",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemTasks_CompletedById",
                table: "OrderItemTasks",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryPersonId",
                table: "Orders",
                column: "DeliveryPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_RestaurantId",
                table: "Orders",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_AddressId",
                table: "Restaurants",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantUsers_UserId",
                table: "RestaurantUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BillingAddressId",
                table: "Users",
                column: "BillingAddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "AddressUser");

            migrationBuilder.DropTable(
                name: "CuisineRestaurant");

            migrationBuilder.DropTable(
                name: "OrderItemCustomizations");

            migrationBuilder.DropTable(
                name: "OrderItemTasks");

            migrationBuilder.DropTable(
                name: "RestaurantUsers");

            migrationBuilder.DropTable(
                name: "Cuisines");

            migrationBuilder.DropTable(
                name: "MenuItemOptionItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "MenuItemOptionLists");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "MenuCategories");

            migrationBuilder.DropTable(
                name: "Restaurants");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
