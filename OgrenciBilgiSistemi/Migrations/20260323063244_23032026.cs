using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class _23032026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ogrenciler_OgrenciVeliler_VeliId",
                table: "Ogrenciler");

            migrationBuilder.DropTable(
                name: "OgrenciVeliler");

            migrationBuilder.CreateTable(
                name: "VeliProfiller",
                columns: table => new
                {
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    VeliAdSoyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VeliTelefon = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    VeliAdres = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    VeliMeslek = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VeliIsYeri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VeliEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VeliYakinlik = table.Column<int>(type: "int", nullable: true),
                    VeliDurum = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeliProfiller", x => x.KullaniciId);
                    table.ForeignKey(
                        name: "FK_VeliProfiller_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Ogrenciler_Kullanicilar_VeliId",
                table: "Ogrenciler",
                column: "VeliId",
                principalTable: "Kullanicilar",
                principalColumn: "KullaniciId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ogrenciler_Kullanicilar_VeliId",
                table: "Ogrenciler");

            migrationBuilder.DropTable(
                name: "VeliProfiller");

            migrationBuilder.CreateTable(
                name: "OgrenciVeliler",
                columns: table => new
                {
                    OgrenciVeliId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: true),
                    VeliAdSoyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VeliAdres = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    VeliDurum = table.Column<bool>(type: "bit", nullable: false),
                    VeliEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VeliIsYeri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VeliMeslek = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VeliTelefon = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    VeliYakinlik = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgrenciVeliler", x => x.OgrenciVeliId);
                    table.ForeignKey(
                        name: "FK_OgrenciVeliler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OgrenciVeliler_KullaniciId",
                table: "OgrenciVeliler",
                column: "KullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ogrenciler_OgrenciVeliler_VeliId",
                table: "Ogrenciler",
                column: "VeliId",
                principalTable: "OgrenciVeliler",
                principalColumn: "OgrenciVeliId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
