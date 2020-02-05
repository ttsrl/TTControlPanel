using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "Licenses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_ApplicationId",
                table: "Licenses",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Applications_ApplicationId",
                table: "Licenses",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Applications_ApplicationId",
                table: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_ApplicationId",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Licenses");
        }
    }
}
