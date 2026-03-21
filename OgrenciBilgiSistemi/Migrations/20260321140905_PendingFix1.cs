using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class PendingFix1 : Migration
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

            migrationBuilder.DropColumn(
                name: "BirimId",
                table: "Kullanicilar");

            migrationBuilder.AddColumn<string>(
                name: "Telefon",
                table: "Kullanicilar",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telefon",
                table: "Kullanicilar");

            migrationBuilder.AddColumn<int>(
                name: "BirimId",
                table: "Kullanicilar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_BirimId",
                table: "Kullanicilar",
                column: "BirimId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Birimler_BirimId",
                table: "Kullanicilar",
                column: "BirimId",
                principalTable: "Birimler",
                principalColumn: "BirimId");
        }
    }
}
