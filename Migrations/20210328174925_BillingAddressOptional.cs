using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace pwr_msi.Migrations {
    public partial class BillingAddressOptional : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Addresses_BillingAddressId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BillingAddressId",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "VerificationTokens",
                columns: table => new {
                    VerificationTokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(name: "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TokenType = table.Column<int>(type: "integer", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                },
                constraints: table => {
                    table.PrimaryKey(name: "PK_VerificationTokens", columns: x => x.VerificationTokenId);
                    table.ForeignKey(
                        name: "FK_VerificationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTokens_UserId",
                table: "VerificationTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Addresses_BillingAddressId",
                table: "Users",
                column: "BillingAddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Addresses_BillingAddressId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "VerificationTokens");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "BillingAddressId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Addresses_BillingAddressId",
                table: "Users",
                column: "BillingAddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
