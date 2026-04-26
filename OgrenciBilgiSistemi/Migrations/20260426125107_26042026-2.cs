using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    /// <inheritdoc />
    public partial class _260420262 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gun",
                table: "OgretmenRandevular");

            migrationBuilder.AddColumn<DateTime>(
                name: "Tarih",
                table: "OgretmenRandevular",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tarih",
                table: "OgretmenRandevular");

            migrationBuilder.AddColumn<int>(
                name: "Gun",
                table: "OgretmenRandevular",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
