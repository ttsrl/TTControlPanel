using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Licenses");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ProductKeys",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Licenses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Banned",
                table: "Licenses",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProductKeys");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "Banned",
                table: "Licenses");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Licenses",
                nullable: false,
                defaultValue: 0);
        }
    }
}
