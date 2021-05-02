using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace pwr_msi.Migrations
{
    public partial class PaymentModelImprovements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "BalanceRepayments");

            migrationBuilder.RenameColumn(
                name: "IsFromBalance",
                table: "Payments",
                newName: "IsTargettingBalance");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<ZonedDateTime>(
                name: "Created",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new NodaTime.ZonedDateTime(NodaTime.Instant.FromUnixTimeTicks(-621355968000000000L), NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ForId("UTC")));

            migrationBuilder.AddColumn<bool>(
                name: "IsBalanceRepayment",
                table: "Payments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ZonedDateTime>(
                name: "Updated",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new NodaTime.ZonedDateTime(NodaTime.Instant.FromUnixTimeTicks(-621355968000000000L), NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ForId("UTC")));

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsBalanceRepayment",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "IsTargettingBalance",
                table: "Payments",
                newName: "IsFromBalance");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "BalanceRepayments",
                columns: table => new
                {
                    BalanceRepaymentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    ExternalRepaymentId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceRepayments", x => x.BalanceRepaymentId);
                    table.ForeignKey(
                        name: "FK_BalanceRepayments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceRepayments_UserId",
                table: "BalanceRepayments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
