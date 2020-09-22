using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoMap.Api.Migrations
{
    public partial class AddedAddedOnColumnToPhotosTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasExternalPhotoUrl",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Photos");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AddedOn",
                table: "Photos",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedOn",
                table: "Photos");

            migrationBuilder.AddColumn<bool>(
                name: "HasExternalPhotoUrl",
                table: "Photos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Photos",
                type: "text",
                nullable: true);
        }
    }
}
