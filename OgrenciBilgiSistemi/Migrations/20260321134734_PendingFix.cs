using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class PendingFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonelId",
                table: "Kullanicilar",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 19,
                column: "Baslik",
                value: "Öğrenci Giriş Çıkış Raporları");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 22,
                column: "Baslik",
                value: "Öğrenci Ziyaretçi Raporu");

            migrationBuilder.CreateIndex(
                name: "UX_Kullanicilar_PersonelId",
                table: "Kullanicilar",
                column: "PersonelId",
                unique: true,
                filter: "[PersonelId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Personeller_PersonelId",
                table: "Kullanicilar",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "PersonelId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_Personeller_PersonelId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "UX_Kullanicilar_PersonelId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "PersonelId",
                table: "Kullanicilar");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 19,
                column: "Baslik",
                value: "Öğrenci Giriş Çıkış");

            migrationBuilder.UpdateData(
                table: "MenuOgeler",
                keyColumn: "Id",
                keyValue: 22,
                column: "Baslik",
                value: "Öğrenci Ziyaretçi");
        }
    }
}
