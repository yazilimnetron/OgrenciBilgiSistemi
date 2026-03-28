using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class _2803206 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Ziyaretciler_CihazId",
                table: "Ziyaretciler",
                column: "CihazId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_Rol",
                table: "Kullanicilar",
                column: "Rol");

            migrationBuilder.AddForeignKey(
                name: "FK_Ziyaretciler_Cihazlar_CihazId",
                table: "Ziyaretciler",
                column: "CihazId",
                principalTable: "Cihazlar",
                principalColumn: "CihazId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ziyaretciler_Cihazlar_CihazId",
                table: "Ziyaretciler");

            migrationBuilder.DropIndex(
                name: "IX_Ziyaretciler_CihazId",
                table: "Ziyaretciler");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_Rol",
                table: "Kullanicilar");
        }
    }
}
