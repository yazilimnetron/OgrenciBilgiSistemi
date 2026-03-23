using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class _23032027 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ogrenciler_Servisler_ServisId",
                table: "Ogrenciler");

            migrationBuilder.DropForeignKey(
                name: "FK_ServisYoklamalar_Servisler_ServisId",
                table: "ServisYoklamalar");

            migrationBuilder.DropTable(
                name: "Servisler");

            migrationBuilder.DropIndex(
                name: "IX_ServisYoklamalar_KullaniciId",
                table: "ServisYoklamalar");

            migrationBuilder.DropIndex(
                name: "IX_ServisYoklamalar_ServisId_OgrenciId_Periyot_OlusturulmaTarihi",
                table: "ServisYoklamalar");

            migrationBuilder.DropColumn(
                name: "ServisId",
                table: "ServisYoklamalar");

            migrationBuilder.CreateTable(
                name: "ServisProfiller",
                columns: table => new
                {
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Plaka = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoforTelefon = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ServisDurum = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServisProfiller", x => x.KullaniciId);
                    table.ForeignKey(
                        name: "FK_ServisProfiller_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServisYoklamalar_KullaniciId_OgrenciId_Periyot_OlusturulmaTarihi",
                table: "ServisYoklamalar",
                columns: new[] { "KullaniciId", "OgrenciId", "Periyot", "OlusturulmaTarihi" });

            migrationBuilder.AddForeignKey(
                name: "FK_Ogrenciler_Kullanicilar_ServisId",
                table: "Ogrenciler",
                column: "ServisId",
                principalTable: "Kullanicilar",
                principalColumn: "KullaniciId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ogrenciler_Kullanicilar_ServisId",
                table: "Ogrenciler");

            migrationBuilder.DropTable(
                name: "ServisProfiller");

            migrationBuilder.DropIndex(
                name: "IX_ServisYoklamalar_KullaniciId_OgrenciId_Periyot_OlusturulmaTarihi",
                table: "ServisYoklamalar");

            migrationBuilder.AddColumn<int>(
                name: "ServisId",
                table: "ServisYoklamalar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Servisler",
                columns: table => new
                {
                    ServisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: true),
                    Plaka = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ServisDurum = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servisler", x => x.ServisId);
                    table.ForeignKey(
                        name: "FK_Servisler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServisYoklamalar_KullaniciId",
                table: "ServisYoklamalar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ServisYoklamalar_ServisId_OgrenciId_Periyot_OlusturulmaTarihi",
                table: "ServisYoklamalar",
                columns: new[] { "ServisId", "OgrenciId", "Periyot", "OlusturulmaTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_Servisler_KullaniciId",
                table: "Servisler",
                column: "KullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ogrenciler_Servisler_ServisId",
                table: "Ogrenciler",
                column: "ServisId",
                principalTable: "Servisler",
                principalColumn: "ServisId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ServisYoklamalar_Servisler_ServisId",
                table: "ServisYoklamalar",
                column: "ServisId",
                principalTable: "Servisler",
                principalColumn: "ServisId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
