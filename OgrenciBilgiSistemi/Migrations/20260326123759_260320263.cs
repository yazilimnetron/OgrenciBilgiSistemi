using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class _260320263 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KitapDetaylar_Kitaplar_KitapId",
                table: "KitapDetaylar");

            migrationBuilder.AddForeignKey(
                name: "FK_KitapDetaylar_Kitaplar_KitapId",
                table: "KitapDetaylar",
                column: "KitapId",
                principalTable: "Kitaplar",
                principalColumn: "KitapId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KitapDetaylar_Kitaplar_KitapId",
                table: "KitapDetaylar");

            migrationBuilder.AddForeignKey(
                name: "FK_KitapDetaylar_Kitaplar_KitapId",
                table: "KitapDetaylar",
                column: "KitapId",
                principalTable: "Kitaplar",
                principalColumn: "KitapId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
