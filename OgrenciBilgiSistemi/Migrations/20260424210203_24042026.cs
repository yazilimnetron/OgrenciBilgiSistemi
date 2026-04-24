using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class _24042026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "KullaniciAdi",
                table: "Kullanicilar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "OgretmenMusaitlikler",
                columns: table => new
                {
                    MusaitlikId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OgretmenKullaniciId = table.Column<int>(type: "int", nullable: false),
                    Gun = table.Column<int>(type: "int", nullable: false),
                    BaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "time", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Randevular",
                columns: table => new
                {
                    RandevuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OgretmenKullaniciId = table.Column<int>(type: "int", nullable: false),
                    VeliKullaniciId = table.Column<int>(type: "int", nullable: false),
                    OgrenciId = table.Column<int>(type: "int", nullable: true),
                    RandevuTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SureDakika = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Not = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OgretmenTarafindanOlusturuldu = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Randevular", x => x.RandevuId);
                    table.ForeignKey(
                        name: "FK_Randevular_Kullanicilar_OgretmenKullaniciId",
                        column: x => x.OgretmenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Randevular_Kullanicilar_VeliKullaniciId",
                        column: x => x.VeliKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Randevular_Ogrenciler_OgrenciId",
                        column: x => x.OgrenciId,
                        principalTable: "Ogrenciler",
                        principalColumn: "OgrenciId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    BildirimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AliciKullaniciId = table.Column<int>(type: "int", nullable: false),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RandevuId = table.Column<int>(type: "int", nullable: true),
                    Okundu = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.BildirimId);
                    table.ForeignKey(
                        name: "FK_Bildirimler_Kullanicilar_AliciKullaniciId",
                        column: x => x.AliciKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bildirimler_Randevular_RandevuId",
                        column: x => x.RandevuId,
                        principalTable: "Randevular",
                        principalColumn: "RandevuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MenuOgeler",
                columns: new[] { "Id", "Action", "AnaMenuId", "Baslik", "Controller", "GerekliRole", "Sirala" },
                values: new object[,]
                {
                    { 30, null, null, "Randevular", null, null, 12 },
                    { 31, "Index", 30, "Randevu Listesi", "Randevular", null, 1 },
                    { 32, "Index", 30, "Öğretmen Müsaitlik", "OgretmenMusaitlik", null, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "UX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_Alici_Okundu",
                table: "Bildirimler",
                columns: new[] { "AliciKullaniciId", "Okundu" });

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_RandevuId",
                table: "Bildirimler",
                column: "RandevuId");

            migrationBuilder.CreateIndex(
                name: "IX_OgretmenMusaitlikler_OgretmenKullaniciId",
                table: "OgretmenMusaitlikler",
                column: "OgretmenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_OgrenciId",
                table: "Randevular",
                column: "OgrenciId");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_OgretmenKullaniciId",
                table: "Randevular",
                column: "OgretmenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_Tarih",
                table: "Randevular",
                column: "RandevuTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_VeliKullaniciId",
                table: "Randevular",
                column: "VeliKullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropTable(
                name: "OgretmenMusaitlikler");

            migrationBuilder.DropTable(
                name: "Randevular");

            migrationBuilder.DropIndex(
                name: "UX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar");

            migrationBuilder.DeleteData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.AlterColumn<string>(
                name: "KullaniciAdi",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
