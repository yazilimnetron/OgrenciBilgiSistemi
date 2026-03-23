using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class _220320268 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServisYoklamalar",
                columns: table => new
                {
                    ServisYoklamaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OgrenciId = table.Column<int>(type: "int", nullable: false),
                    ServisId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    DurumId = table.Column<int>(type: "int", nullable: false),
                    Periyot = table.Column<int>(type: "int", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServisYoklamalar", x => x.ServisYoklamaId);
                    table.ForeignKey(
                        name: "FK_ServisYoklamalar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServisYoklamalar_Ogrenciler_OgrenciId",
                        column: x => x.OgrenciId,
                        principalTable: "Ogrenciler",
                        principalColumn: "OgrenciId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServisYoklamalar_Servisler_ServisId",
                        column: x => x.ServisId,
                        principalTable: "Servisler",
                        principalColumn: "ServisId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServisYoklamalar_KullaniciId",
                table: "ServisYoklamalar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ServisYoklamalar_OgrenciId",
                table: "ServisYoklamalar",
                column: "OgrenciId");

            migrationBuilder.CreateIndex(
                name: "IX_ServisYoklamalar_ServisId_OgrenciId_Periyot_OlusturulmaTarihi",
                table: "ServisYoklamalar",
                columns: new[] { "ServisId", "OgrenciId", "Periyot", "OlusturulmaTarihi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServisYoklamalar");
        }
    }
}
