using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestCode",
                table: "Licenses",
                newName: "ProductKey");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationVersionId",
                table: "Licenses",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationsVersions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Version = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    ApplicationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationsVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationsVersions_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_ApplicationVersionId",
                table: "Licenses",
                column: "ApplicationVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationsVersions_ApplicationId",
                table: "ApplicationsVersions",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_ApplicationsVersions_ApplicationVersionId",
                table: "Licenses",
                column: "ApplicationVersionId",
                principalTable: "ApplicationsVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_ApplicationsVersions_ApplicationVersionId",
                table: "Licenses");

            migrationBuilder.DropTable(
                name: "ApplicationsVersions");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_ApplicationVersionId",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "ApplicationVersionId",
                table: "Licenses");

            migrationBuilder.RenameColumn(
                name: "ProductKey",
                table: "Licenses",
                newName: "RequestCode");
        }
    }
}
