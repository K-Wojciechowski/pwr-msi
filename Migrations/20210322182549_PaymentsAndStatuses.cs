using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace pwr_msi.Migrations {
    public partial class PaymentsAndStatuses : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "OrderItemTasks");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Users",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BalanceRepayments",
                columns: table => new {
                    BalanceRepaymentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalRepaymentId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_BalanceRepayments", columns: x => x.BalanceRepaymentId);
                    table.ForeignKey(
                        name: "FK_BalanceRepayments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTasks",
                columns: table => new {
                    OrderTaskId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Task = table.Column<int>(type: "integer", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CompletedById = table.Column<int>(type: "integer", nullable: false),
                    AssigneeUserId = table.Column<int>(type: "integer", nullable: false),
                    AssigneeRestaurantId = table.Column<int>(type: "integer", nullable: false),
                    AssigneeType = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_OrderTasks", columns: x => x.OrderTaskId);
                    table.ForeignKey(
                        name: "FK_OrderTasks_Restaurants_AssigneeRestaurantId",
                        column: x => x.AssigneeRestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTasks_Users_AssigneeUserId",
                        column: x => x.AssigneeUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTasks_Users_CompletedById",
                        column: x => x.CompletedById,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new {
                    PaymentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalPaymentId = table.Column<string>(type: "text", nullable: true),
                    IsReturn = table.Column<bool>(type: "boolean", nullable: false),
                    IsFromBalance = table.Column<bool>(type: "boolean", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_Payments", columns: x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceRepayments_UserId",
                table: "BalanceRepayments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTasks_AssigneeRestaurantId",
                table: "OrderTasks",
                column: "AssigneeRestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTasks_AssigneeUserId",
                table: "OrderTasks",
                column: "AssigneeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTasks_CompletedById",
                table: "OrderTasks",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "BalanceRepayments");

            migrationBuilder.DropTable(
                name: "OrderTasks");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.CreateTable(
                name: "OrderItemTasks",
                columns: table => new {
                    OrderItemTaskId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompletedById = table.Column<int>(type: "integer", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Task = table.Column<int>(type: "integer", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemTasks_CompletedById",
                table: "OrderItemTasks",
                column: "CompletedById");
        }
    }
}
