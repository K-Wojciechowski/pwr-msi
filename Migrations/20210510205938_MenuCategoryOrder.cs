using Microsoft.EntityFrameworkCore.Migrations;

namespace pwr_msi.Migrations
{
    public partial class MenuCategoryOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuCategoryOrder",
                table: "MenuCategories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuCategoryOrder",
                table: "MenuCategories");
        }
    }
}
