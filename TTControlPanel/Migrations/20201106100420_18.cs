using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AtProgress",
                table: "Workings");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Workings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Workings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Workings");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Workings");

            migrationBuilder.AddColumn<bool>(
                name: "AtProgress",
                table: "Workings",
                nullable: false,
                defaultValue: false);
        }
    }
}
