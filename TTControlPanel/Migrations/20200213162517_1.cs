using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activate",
                table: "Licenses");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Licenses",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Licenses");

            migrationBuilder.AddColumn<bool>(
                name: "Activate",
                table: "Licenses",
                nullable: false,
                defaultValue: false);
        }
    }
}
