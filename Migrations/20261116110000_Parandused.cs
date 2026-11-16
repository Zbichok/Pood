using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tooted.Migrations
{
    /// <summary>Pilt ja Kirjeldus on valikulised (NULL lubatud), e-post on unikaalne.</summary>
    [DbContext(typeof(ToodedContext))]
    [Migration("20261116110000_Parandused")]
    public partial class Parandused : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Pilt", table: "Toodetabel",
                type: "nvarchar(max)", nullable: true,
                oldClrType: typeof(string), oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Kirjeldus", table: "Kategooriatabel",
                type: "nvarchar(max)", nullable: true,
                oldClrType: typeof(string), oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email", table: "Kasutajad",
                type: "nvarchar(450)", maxLength: 450, nullable: false,
                oldClrType: typeof(string), oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Kasutajad_Email", table: "Kasutajad",
                column: "Email", unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Kasutajad_Email", table: "Kasutajad");

            migrationBuilder.AlterColumn<string>(
                name: "Email", table: "Kasutajad",
                type: "nvarchar(max)", nullable: false,
                oldClrType: typeof(string), oldType: "nvarchar(450)", oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "Kirjeldus", table: "Kategooriatabel",
                type: "nvarchar(max)", nullable: false, defaultValue: "",
                oldClrType: typeof(string), oldType: "nvarchar(max)", oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Pilt", table: "Toodetabel",
                type: "nvarchar(max)", nullable: false, defaultValue: "",
                oldClrType: typeof(string), oldType: "nvarchar(max)", oldNullable: true);
        }
    }
}
