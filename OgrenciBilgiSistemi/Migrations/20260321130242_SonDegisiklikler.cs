using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class SonDegisiklikler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VeliAdSoyad",
                table: "OgrenciVeliler",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "OgrenciKartNo",
                table: "Ogrenciler",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "KullaniciAdi",
                table: "Kullanicilar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "OgrenciVeliId",
                table: "Kullanicilar",
                type: "int",
                nullable: true);


            migrationBuilder.CreateTable(
                name: "SinifYoklamaDurumlar",
                columns: table => new
                {
                    DurumId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DurumAd = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinifYoklamaDurumlar", x => x.DurumId);
                });

            migrationBuilder.CreateTable(
                name: "SinifYoklamalar",
                columns: table => new
                {
                    SinifYoklamaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OgrenciId = table.Column<int>(type: "int", nullable: false),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    Ders1 = table.Column<int>(type: "int", nullable: true),
                    Ders2 = table.Column<int>(type: "int", nullable: true),
                    Ders3 = table.Column<int>(type: "int", nullable: true),
                    Ders4 = table.Column<int>(type: "int", nullable: true),
                    Ders5 = table.Column<int>(type: "int", nullable: true),
                    Ders6 = table.Column<int>(type: "int", nullable: true),
                    Ders7 = table.Column<int>(type: "int", nullable: true),
                    Ders8 = table.Column<int>(type: "int", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinifYoklamalar", x => x.SinifYoklamaId);
                    table.ForeignKey(
                        name: "FK_SinifYoklamalar_Ogrenciler_OgrenciId",
                        column: x => x.OgrenciId,
                        principalTable: "Ogrenciler",
                        principalColumn: "OgrenciId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SinifYoklamalar_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "PersonelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 7,
                column: "Controller",
                value: "Aidat");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 8,
                column: "Controller",
                value: "Yemekhane");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 10,
                column: "Controller",
                value: "Ziyaretciler");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 21,
                column: "Controller",
                value: "Aidat");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 22,
                column: "Controller",
                value: "Ziyaretciler");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 23,
                column: "Controller",
                value: "Yemekhane");

            migrationBuilder.InsertData(
                table: "MenuOgeler",
                columns: new[] { "Id", "Action", "AnaMenuId", "Baslik", "Controller", "GerekliRole", "Sirala" },
                values: new object[] { 26, null, null, "Servisler", null, null, 10 });

            migrationBuilder.InsertData(
                table: "SinifYoklamaDurumlar",
                columns: new[] { "DurumId", "DurumAd" },
                values: new object[,]
                {
                    { 1, "Var" },
                    { 2, "Yok" },
                    { 3, "Geç" },
                    { 4, "İzinli" },
                    { 5, "Raporlu" }
                });

            migrationBuilder.InsertData(
                table: "MenuOgeler",
                columns: new[] { "Id", "Action", "AnaMenuId", "Baslik", "Controller", "GerekliRole", "Sirala" },
                values: new object[] { 27, "Index", 26, "Servis Listesi", "Servisler", null, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Ogrenciler_ServisId",
                table: "Ogrenciler",
                column: "ServisId");

            migrationBuilder.CreateIndex(
                name: "UX_Ogrenciler_OgrenciKartNo",
                table: "Ogrenciler",
                column: "OgrenciKartNo",
                unique: true,
                filter: "[OgrenciKartNo] IS NOT NULL AND [OgrenciKartNo] != ''");

            migrationBuilder.CreateIndex(
                name: "UX_Ogrenciler_OgrenciNo",
                table: "Ogrenciler",
                column: "OgrenciNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_OgrenciVeliId",
                table: "Kullanicilar",
                column: "OgrenciVeliId");

            migrationBuilder.CreateIndex(
                name: "UX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servisler_KullaniciId",
                table: "Servisler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_SinifYoklamalar_OgrenciId_OlusturulmaTarihi",
                table: "SinifYoklamalar",
                columns: new[] { "OgrenciId", "OlusturulmaTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_SinifYoklamalar_PersonelId",
                table: "SinifYoklamalar",
                column: "PersonelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_OgrenciVeliler_OgrenciVeliId",
                table: "Kullanicilar",
                column: "OgrenciVeliId",
                principalTable: "OgrenciVeliler",
                principalColumn: "OgrenciVeliId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Ogrenciler_Servisler_ServisId",
                table: "Ogrenciler",
                column: "ServisId",
                principalTable: "Servisler",
                principalColumn: "ServisId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_OgrenciVeliler_OgrenciVeliId",
                table: "Kullanicilar");

            migrationBuilder.DropForeignKey(
                name: "FK_Ogrenciler_Servisler_ServisId",
                table: "Ogrenciler");

            migrationBuilder.DropTable(
                name: "Servisler");

            migrationBuilder.DropTable(
                name: "SinifYoklamaDurumlar");

            migrationBuilder.DropTable(
                name: "SinifYoklamalar");

            migrationBuilder.DropIndex(
                name: "IX_Ogrenciler_ServisId",
                table: "Ogrenciler");

            migrationBuilder.DropIndex(
                name: "UX_Ogrenciler_OgrenciKartNo",
                table: "Ogrenciler");

            migrationBuilder.DropIndex(
                name: "UX_Ogrenciler_OgrenciNo",
                table: "Ogrenciler");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_OgrenciVeliId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "UX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar");

            migrationBuilder.DeleteData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DropColumn(
                name: "ServisId",
                table: "Ogrenciler");

            migrationBuilder.DropColumn(
                name: "OgrenciVeliId",
                table: "Kullanicilar");

            migrationBuilder.AlterColumn<string>(
                name: "VeliAdSoyad",
                table: "OgrenciVeliler",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OgrenciKartNo",
                table: "Ogrenciler",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KullaniciAdi",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 7,
                column: "Controller",
                value: "OgrenciAidat");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 8,
                column: "Controller",
                value: "OgrenciYemekhane");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 10,
                column: "Controller",
                value: "Ziyaretci");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 21,
                column: "Controller",
                value: "OgrenciAidat");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 22,
                column: "Controller",
                value: "Ziyaretci");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 23,
                column: "Controller",
                value: "OgrenciYemekhane");
        }
    }
}
