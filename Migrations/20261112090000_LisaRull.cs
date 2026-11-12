using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tooted.Migrations
{
    /// <summary>Päevase rulli veerud tabelisse Kasutajad.</summary>
    [DbContext(typeof(ToodedContext))]
    [Migration("20261112090000_LisaRull")]
    public partial class LisaRull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Saldo", table: "Kasutajad",
                type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Soodustus", table: "Kasutajad",
                type: "int", nullable: false, defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TasutaKohaletoimetus", table: "Kasutajad",
                type: "bit", nullable: false, defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TasutaToodeId", table: "Kasutajad",
                type: "int", nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ViimanePoore", table: "Kasutajad",
                type: "datetime2", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Saldo", table: "Kasutajad");
            migrationBuilder.DropColumn(name: "Soodustus", table: "Kasutajad");
            migrationBuilder.DropColumn(name: "TasutaKohaletoimetus", table: "Kasutajad");
            migrationBuilder.DropColumn(name: "TasutaToodeId", table: "Kasutajad");
            migrationBuilder.DropColumn(name: "ViimanePoore", table: "Kasutajad");
        }
    }
}
