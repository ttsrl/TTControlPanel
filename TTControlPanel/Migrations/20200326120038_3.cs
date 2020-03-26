using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddedUserId",
                table: "Hids",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hids_AddedUserId",
                table: "Hids",
                column: "AddedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hids_Users_AddedUserId",
                table: "Hids",
                column: "AddedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hids_Users_AddedUserId",
                table: "Hids");

            migrationBuilder.DropIndex(
                name: "IX_Hids_AddedUserId",
                table: "Hids");

            migrationBuilder.DropColumn(
                name: "AddedUserId",
                table: "Hids");
        }
    }
}
