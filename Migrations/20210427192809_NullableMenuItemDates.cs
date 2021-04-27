using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace pwr_msi.Migrations
{
    public partial class NullableMenuItemDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "ValidUntil",
                table: "MenuItems",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "ValidUntil",
                table: "MenuCategories",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "ValidUntil",
                table: "MenuItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new NodaTime.ZonedDateTime(NodaTime.Instant.FromUnixTimeTicks(-621355968000000000L), NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ForId("UTC")),
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "ValidUntil",
                table: "MenuCategories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new NodaTime.ZonedDateTime(NodaTime.Instant.FromUnixTimeTicks(-621355968000000000L), NodaTime.TimeZones.TzdbDateTimeZoneSource.Default.ForId("UTC")),
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
