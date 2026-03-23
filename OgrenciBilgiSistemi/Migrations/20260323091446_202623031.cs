using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class _202623031 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_Birimler_BirimId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_BirimId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "UX_Kullanicilar_KartNo",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "BirimId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "GorselPath",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "KartNo",
                table: "Kullanicilar");

            migrationBuilder.CreateTable(
                name: "OgretmenProfiller",
                columns: table => new
                {
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    BirimId = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    GorselPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OgretmenDurum = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgretmenProfiller", x => x.KullaniciId);
                    table.ForeignKey(
                        name: "FK_OgretmenProfiller_Birimler_BirimId",
                        column: x => x.BirimId,
                        principalTable: "Birimler",
                        principalColumn: "BirimId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OgretmenProfiller_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 2,
                column: "Baslik",
                value: "Öğretmenler");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 2, "Öğretmen İşlemleri", "Ogretmenler", 2 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Öğrenciler", null, 3 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { 5, "Öğrenci İşlemleri", "Ogrenciler", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { 5, "Aidat İşlemleri", "Aidat", 2 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 5, "Yemekhane İşlemleri", "Yemekhane", 3 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Ziyaretçiler", null, 4 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 9, "Ziyaretçi İşlemleri", "Ziyaretciler", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Kullanıcılar", null, 5 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 11, "Kullanıcı Listesi", "Kullanicilar", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Kitaplar", null, 6 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { 13, "Kitap Listesi", "Kitaplar", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 13, "Kitap Hareketleri", "KitapDetaylar", 2 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Cihazlar", null, 7 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 16, "Cihaz Listesi", "Cihazlar", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Raporlar", null, 8 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Detay", 18, "Öğrenci Giriş Çıkış Raporları", "OgrenciGirisCikis", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "OgrenciVeliRapor", 18, "Öğrenci Veli Raporu", "Ogrenciler", 2 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "AidatRapor", 18, "Öğrenci Aidat Raporu", "Aidat", 3 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "ZiyaretciRapor", 18, "Öğrenci Ziyaretçi Raporu", "Ziyaretciler", 4 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "YemekRapor", 18, "Öğrenci Yemek Raporu", "Yemekhane", 5 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Kart Oku", null, 9 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 24, "Kart Okuma Ekranı", "KartOku", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Servisler", null, 10 });

            migrationBuilder.InsertData(
                table: "MenuOgeler",
                columns: new[] { "Id", "Action", "AnaMenuId", "Baslik", "Controller", "GerekliRole", "Sirala" },
                values: new object[] { 27, "Index", 26, "Servis Listesi", "Servisler", null, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_OgretmenProfiller_BirimId",
                table: "OgretmenProfiller",
                column: "BirimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OgretmenProfiller");

            migrationBuilder.DeleteData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.AddColumn<int>(
                name: "BirimId",
                table: "Kullanicilar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Kullanicilar",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GorselPath",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KartNo",
                table: "Kullanicilar",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 2,
                column: "Baslik",
                value: "Birimler");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Öğrenciler", null, 3 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 4, "Öğrenci İşlemleri", "Ogrenciler", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { 4, "Aidat İşlemleri", "Aidat", 2 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { 4, "Yemekhane İşlemleri", "Yemekhane", 3 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Ziyaretçiler", null, 4 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 8, "Ziyaretçi İşlemleri", "Ziyaretciler", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Kullanıcılar", null, 5 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 10, "Kullanıcı Listesi", "Kullanicilar", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Kitaplar", null, 6 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 12, "Kitap Listesi", "Kitaplar", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { 12, "Kitap Hareketleri", "KitapDetaylar", 2 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Cihazlar", null, 7 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 15, "Cihaz Listesi", "Cihazlar", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Raporlar", null, 8 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Detay", 17, "Öğrenci Giriş Çıkış Raporları", "OgrenciGirisCikis", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "OgrenciVeliRapor", 17, "Öğrenci Veli Raporu", "Ogrenciler", 2 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "AidatRapor", 17, "Öğrenci Aidat Raporu", "Aidat", 3 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "ZiyaretciRapor", 17, "Öğrenci Ziyaretçi Raporu", "Ziyaretciler", 4 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "YemekRapor", 17, "Öğrenci Yemek Raporu", "Yemekhane", 5 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "KartOku", null, 9 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 23, "Kart Okuma Ekranı", "KartOku", 1 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { null, null, "Servisler", null, 10 });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Action", "AnaMenuId", "Baslik", "Controller", "Sirala" },
                values: new object[] { "Index", 25, "Servis Listesi", "Servisler", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_BirimId",
                table: "Kullanicilar",
                column: "BirimId");

            migrationBuilder.CreateIndex(
                name: "UX_Kullanicilar_KartNo",
                table: "Kullanicilar",
                column: "KartNo",
                unique: true,
                filter: "[KartNo] IS NOT NULL AND [KartNo] != ''");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Birimler_BirimId",
                table: "Kullanicilar",
                column: "BirimId",
                principalTable: "Birimler",
                principalColumn: "BirimId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
