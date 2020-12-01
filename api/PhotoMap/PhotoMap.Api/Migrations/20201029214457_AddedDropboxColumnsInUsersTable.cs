using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoMap.Api.Migrations
{
    public partial class AddedDropboxColumnsInUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DropboxStatus",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DropboxToken",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DropboxTokenExpiresOn",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DropboxStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DropboxToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DropboxTokenExpiresOn",
                table: "Users");
        }
    }
}
