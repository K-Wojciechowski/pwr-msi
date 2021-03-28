using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace pwr_msi.Migrations {
    public partial class UseNodaTime : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AlterColumn<Instant>(
                name: "ValidUntil",
                table: "VerificationTokens",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "VerificationTokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "DateCompleted",
                table: "OrderTasks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "ValidUntil",
                table: "MenuItems",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "ValidFrom",
                table: "MenuItems",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "ValidUntil",
                table: "MenuCategories",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "ValidFrom",
                table: "MenuCategories",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "VerificationTokens");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidUntil",
                table: "VerificationTokens",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(Instant),
                oldType: "timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCompleted",
                table: "OrderTasks",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidUntil",
                table: "MenuItems",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "MenuItems",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidUntil",
                table: "MenuCategories",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "MenuCategories",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone");
        }
    }
}
