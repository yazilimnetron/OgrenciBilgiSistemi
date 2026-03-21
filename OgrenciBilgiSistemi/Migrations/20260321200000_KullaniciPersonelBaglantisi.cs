using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciPersonelBaglantisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonelId",
                table: "Kullanicilar",
                type: "int",
                nullable: true);

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
        }
    }
}
