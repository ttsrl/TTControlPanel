using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddedUserId",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddedUserId",
                table: "ApplicationsVersions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LastLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LicenseId = table.Column<int>(nullable: true),
                    Api = table.Column<int>(nullable: false),
                    DateTimeUtc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LastLogs_Licenses_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "Licenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AddedUserId",
                table: "Clients",
                column: "AddedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationsVersions_AddedUserId",
                table: "ApplicationsVersions",
                column: "AddedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LastLogs_LicenseId",
                table: "LastLogs",
                column: "LicenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationsVersions_Users_AddedUserId",
                table: "ApplicationsVersions",
                column: "AddedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Users_AddedUserId",
                table: "Clients",
                column: "AddedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationsVersions_Users_AddedUserId",
                table: "ApplicationsVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Users_AddedUserId",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "LastLogs");

            migrationBuilder.DropIndex(
                name: "IX_Clients_AddedUserId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationsVersions_AddedUserId",
                table: "ApplicationsVersions");

            migrationBuilder.DropColumn(
                name: "AddedUserId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "AddedUserId",
                table: "ApplicationsVersions");
        }
    }
}
