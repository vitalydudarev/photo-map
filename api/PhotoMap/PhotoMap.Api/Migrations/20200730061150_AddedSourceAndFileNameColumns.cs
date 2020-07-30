using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoMap.Api.Migrations
{
    public partial class AddedSourceAndFileNameColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Photos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Photos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Photos");
        }
    }
}
