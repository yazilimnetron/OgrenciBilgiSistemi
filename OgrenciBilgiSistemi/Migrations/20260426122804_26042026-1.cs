using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class _260420261 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OgretmenMusaitlikler");

            migrationBuilder.CreateTable(
                name: "OgretmenRandevular",
                columns: table => new
                {
                    OgretmenRandevuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OgretmenKullaniciId = table.Column<int>(type: "int", nullable: false),
                    Gun = table.Column<int>(type: "int", nullable: false),
                    BaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgretmenRandevular", x => x.OgretmenRandevuId);
                    table.ForeignKey(
                        name: "FK_OgretmenRandevular_Kullanicilar_OgretmenKullaniciId",
                        column: x => x.OgretmenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Baslik", "Controller" },
                values: new object[] { "Öğretmen Randevu Takvimi", "OgretmenRandevu" });

            migrationBuilder.CreateIndex(
                name: "IX_OgretmenRandevular_OgretmenKullaniciId",
                table: "OgretmenRandevular",
                column: "OgretmenKullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OgretmenRandevular");

            migrationBuilder.CreateTable(
                name: "OgretmenMusaitlikler",
                columns: table => new
                {
                    MusaitlikId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OgretmenKullaniciId = table.Column<int>(type: "int", nullable: false),
                    BaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    Gun = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgretmenMusaitlikler", x => x.MusaitlikId);
                    table.ForeignKey(
                        name: "FK_OgretmenMusaitlikler_Kullanicilar_OgretmenKullaniciId",
                        column: x => x.OgretmenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Baslik", "Controller" },
                values: new object[] { "Öğretmen Müsaitlik", "OgretmenMusaitlik" });

            migrationBuilder.CreateIndex(
                name: "IX_OgretmenMusaitlikler_OgretmenKullaniciId",
                table: "OgretmenMusaitlikler",
                column: "OgretmenKullaniciId");
        }
    }
}
