using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace pwr_msi.Migrations
{
    public partial class OrderDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderTasks_Restaurants_AssigneeRestaurantId",
                table: "OrderTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTasks_Users_AssigneeUserId",
                table: "OrderTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTasks_Users_CompletedById",
                table: "OrderTasks");

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "DateCompleted",
                table: "OrderTasks",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "CompletedById",
                table: "OrderTasks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AssigneeUserId",
                table: "OrderTasks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AssigneeType",
                table: "OrderTasks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AssigneeRestaurantId",
                table: "OrderTasks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "OrderTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<ZonedDateTime>(
                name: "Created",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new NodaTime.ZonedDateTime(NodaTime.Instant.FromUnixTimeTicks(-621355968000000000L), NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ForId("UTC")));

            migrationBuilder.AddColumn<ZonedDateTime>(
                name: "Delivered",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<ZonedDateTime>(
                name: "Updated",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new NodaTime.ZonedDateTime(NodaTime.Instant.FromUnixTimeTicks(-621355968000000000L), NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ForId("UTC")));

            migrationBuilder.CreateIndex(
                name: "IX_OrderTasks_OrderId",
                table: "OrderTasks",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTasks_Orders_OrderId",
                table: "OrderTasks",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTasks_Restaurants_AssigneeRestaurantId",
                table: "OrderTasks",
                column: "AssigneeRestaurantId",
                principalTable: "Restaurants",
                principalColumn: "RestaurantId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTasks_Users_AssigneeUserId",
                table: "OrderTasks",
                column: "AssigneeUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTasks_Users_CompletedById",
                table: "OrderTasks",
                column: "CompletedById",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderTasks_Orders_OrderId",
                table: "OrderTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTasks_Restaurants_AssigneeRestaurantId",
                table: "OrderTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTasks_Users_AssigneeUserId",
                table: "OrderTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTasks_Users_CompletedById",
                table: "OrderTasks");

            migrationBuilder.DropIndex(
                name: "IX_OrderTasks_OrderId",
                table: "OrderTasks");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "OrderTasks");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Delivered",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Orders");

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "DateCompleted",
                table: "OrderTasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new NodaTime.ZonedDateTime(NodaTime.Instant.FromUnixTimeTicks(-621355968000000000L), NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ForId("UTC")),
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompletedById",
                table: "OrderTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssigneeUserId",
                table: "OrderTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssigneeType",
                table: "OrderTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssigneeRestaurantId",
                table: "OrderTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTasks_Restaurants_AssigneeRestaurantId",
                table: "OrderTasks",
                column: "AssigneeRestaurantId",
                principalTable: "Restaurants",
                principalColumn: "RestaurantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTasks_Users_AssigneeUserId",
                table: "OrderTasks",
                column: "AssigneeUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTasks_Users_CompletedById",
                table: "OrderTasks",
                column: "CompletedById",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
