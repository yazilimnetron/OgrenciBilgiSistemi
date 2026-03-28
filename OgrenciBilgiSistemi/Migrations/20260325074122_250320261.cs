using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class _250320261 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Mevcut VeliTelefon ve SoforTelefon verilerini Kullanicilar.Telefon'a tasi
            migrationBuilder.Sql(@"
                UPDATE K SET K.Telefon = V.VeliTelefon
                FROM Kullanicilar K
                INNER JOIN VeliProfiller V ON V.KullaniciId = K.KullaniciId
                WHERE V.VeliTelefon IS NOT NULL AND K.Telefon IS NULL;

                UPDATE K SET K.Telefon = SP.SoforTelefon
                FROM Kullanicilar K
                INNER JOIN ServisProfiller SP ON SP.KullaniciId = K.KullaniciId
                WHERE SP.SoforTelefon IS NOT NULL AND K.Telefon IS NULL;
            ");

            migrationBuilder.DropColumn(
                name: "VeliAdSoyad",
                table: "VeliProfiller");

            migrationBuilder.DropColumn(
                name: "VeliTelefon",
                table: "VeliProfiller");

            migrationBuilder.DropColumn(
                name: "SoforTelefon",
                table: "ServisProfiller");

            migrationBuilder.DropColumn(
                name: "VeliAdSoyad",
                table: "ServisProfiller");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VeliAdSoyad",
                table: "VeliProfiller",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VeliTelefon",
                table: "VeliProfiller",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoforTelefon",
                table: "ServisProfiller",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VeliAdSoyad",
                table: "ServisProfiller",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
