using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tooted.Migrations
{
    /// <summary>SQLite andmebaasi algne skeem: kategooriad, tooted ja kasutajad.</summary>
    [DbContext(typeof(ToodedContext))]
    [Migration("20261116120000_InitialCreate")]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kasutajad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nimi = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    ParoolHash = table.Column<string>(type: "TEXT", nullable: false),
                    Roll = table.Column<string>(type: "TEXT", nullable: false),
                    Saldo = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    ViimanePoore = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Soodustus = table.Column<int>(type: "INTEGER", nullable: false),
                    TasutaKohaletoimetus = table.Column<bool>(type: "INTEGER", nullable: false),
                    TasutaToodeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kasutajad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategooriatabel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Kategooria_nimetus = table.Column<string>(type: "TEXT", nullable: false),
                    Kirjeldus = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategooriatabel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Toodetabel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Toodenimetus = table.Column<string>(type: "TEXT", nullable: false),
                    Kogus = table.Column<int>(type: "INTEGER", nullable: false),
                    Hind = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Pilt = table.Column<string>(type: "TEXT", nullable: true),
                    KategooriaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toodetabel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Toodetabel_Kategooriatabel_KategooriaId",
                        column: x => x.KategooriaId,
                        principalTable: "Kategooriatabel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kasutajad_Email",
                table: "Kasutajad",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Toodetabel_KategooriaId",
                table: "Toodetabel",
                column: "KategooriaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Kasutajad");
            migrationBuilder.DropTable(name: "Toodetabel");
            migrationBuilder.DropTable(name: "Kategooriatabel");
        }
    }
}
