using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoMap.Api.Migrations
{
    public partial class UpdateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateTimeTaken",
                table: "Photo",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ExifString",
                table: "Photo",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasGps",
                table: "Photo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Photo",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Photo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateTimeTaken",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "ExifString",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "HasGps",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Photo");
        }
    }
}
