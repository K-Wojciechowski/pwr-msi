using Microsoft.EntityFrameworkCore.Migrations;

namespace pwr_msi.Migrations
{
    public partial class RestoreAddressLines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Addresses",
                newName: "SecondLine");

            migrationBuilder.RenameColumn(
                name: "HouseNumber",
                table: "Addresses",
                newName: "FirstLine");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecondLine",
                table: "Addresses",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "FirstLine",
                table: "Addresses",
                newName: "HouseNumber");
        }
    }
}
