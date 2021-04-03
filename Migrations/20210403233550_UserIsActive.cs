using Microsoft.EntityFrameworkCore.Migrations;

namespace pwr_msi.Migrations {
    public partial class UserIsActive : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");
        }
    }
}
