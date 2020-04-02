using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visible",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "Ban",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ban",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "Users",
                nullable: false,
                defaultValue: true);
        }
    }
}
