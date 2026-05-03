using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class DuyuruOkumalariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DuyuruOkumalari",
                columns: table => new
                {
                    DuyuruOkumaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DuyuruId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    OkunduTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuyuruOkumalari", x => x.DuyuruOkumaId);
                    table.ForeignKey(
                        name: "FK_DuyuruOkumalari_Duyurular_DuyuruId",
                        column: x => x.DuyuruId,
                        principalTable: "Duyurular",
                        principalColumn: "DuyuruId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DuyuruOkumalari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DuyuruOkumalar_Duyuru_Kullanici_Unique",
                table: "DuyuruOkumalari",
                columns: new[] { "DuyuruId", "KullaniciId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DuyuruOkumalar_Kullanici",
                table: "DuyuruOkumalari",
                column: "KullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuyuruOkumalari");
        }
    }
}
