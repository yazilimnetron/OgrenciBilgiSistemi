using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciVeliBaglantisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OgrenciVeliId",
                table: "Kullanicilar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_OgrenciVeliId",
                table: "Kullanicilar",
                column: "OgrenciVeliId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_OgrenciVeliler_OgrenciVeliId",
                table: "Kullanicilar",
                column: "OgrenciVeliId",
                principalTable: "OgrenciVeliler",
                principalColumn: "OgrenciVeliId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_OgrenciVeliler_OgrenciVeliId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_OgrenciVeliId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "OgrenciVeliId",
                table: "Kullanicilar");
        }
    }
}
