using Microsoft.EntityFrameworkCore.Migrations;

namespace pwr_msi.Migrations
{
    public partial class AddressRestoreLines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        
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
