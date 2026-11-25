using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tooted.Migrations
{
    /// <summary>Eemaldab kasutajate tabelist päevase rulli ja boonuste veerud.</summary>
    [DbContext(typeof(ToodedContext))]
    [Migration("20261125120000_EemaldaRull")]
    public partial class EemaldaRull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Otse SQL: SQLite DropColumn EF-i kaudu vajaks Designer-faili (TargetModel)
            migrationBuilder.Sql("ALTER TABLE \"Kasutajad\" DROP COLUMN \"Saldo\";");
            migrationBuilder.Sql("ALTER TABLE \"Kasutajad\" DROP COLUMN \"ViimanePoore\";");
            migrationBuilder.Sql("ALTER TABLE \"Kasutajad\" DROP COLUMN \"Soodustus\";");
            migrationBuilder.Sql("ALTER TABLE \"Kasutajad\" DROP COLUMN \"TasutaKohaletoimetus\";");
            migrationBuilder.Sql("ALTER TABLE \"Kasutajad\" DROP COLUMN \"TasutaToodeId\";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Saldo", table: "Kasutajad",
                type: "TEXT", precision: 10, scale: 2, nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<DateTime>(
                name: "ViimanePoore", table: "Kasutajad", type: "TEXT", nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "Soodustus", table: "Kasutajad", type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<bool>(
                name: "TasutaKohaletoimetus", table: "Kasutajad", type: "INTEGER", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<int>(
                name: "TasutaToodeId", table: "Kasutajad", type: "INTEGER", nullable: true);
        }
    }
}
