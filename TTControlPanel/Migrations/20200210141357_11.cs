using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hids_Licenses_LicenseId",
                table: "Hids");

            migrationBuilder.DropIndex(
                name: "IX_Hids_LicenseId",
                table: "Hids");

            migrationBuilder.DropColumn(
                name: "LicenseId",
                table: "Hids");

            migrationBuilder.AddColumn<int>(
                name: "HidId",
                table: "Licenses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_HidId",
                table: "Licenses",
                column: "HidId");

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Hids_HidId",
                table: "Licenses",
                column: "HidId",
                principalTable: "Hids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Hids_HidId",
                table: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_HidId",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "HidId",
                table: "Licenses");

            migrationBuilder.AddColumn<int>(
                name: "LicenseId",
                table: "Hids",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hids_LicenseId",
                table: "Hids",
                column: "LicenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hids_Licenses_LicenseId",
                table: "Hids",
                column: "LicenseId",
                principalTable: "Licenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
