using Microsoft.EntityFrameworkCore.Migrations;

namespace pwr_msi.Migrations
{
    public partial class MenuItemOptionOrdering : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuItemOptionListOrder",
                table: "MenuItemOptionLists",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MenuItemOptionItemOrder",
                table: "MenuItemOptionItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuItemOptionListOrder",
                table: "MenuItemOptionLists");

            migrationBuilder.DropColumn(
                name: "MenuItemOptionItemOrder",
                table: "MenuItemOptionItems");
        }
    }
}
