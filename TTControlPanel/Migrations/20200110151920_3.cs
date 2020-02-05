using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenerateDateTime",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "ProductKey",
                table: "Licenses");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Licenses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductKeyId",
                table: "Licenses",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductKeys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(nullable: true),
                    GenerateUserId = table.Column<int>(nullable: true),
                    GenerateDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductKeys_Users_GenerateUserId",
                        column: x => x.GenerateUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_ClientId",
                table: "Licenses",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_ProductKeyId",
                table: "Licenses",
                column: "ProductKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductKeys_GenerateUserId",
                table: "ProductKeys",
                column: "GenerateUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Clients_ClientId",
                table: "Licenses",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_ProductKeys_ProductKeyId",
                table: "Licenses",
                column: "ProductKeyId",
                principalTable: "ProductKeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Clients_ClientId",
                table: "Licenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_ProductKeys_ProductKeyId",
                table: "Licenses");

            migrationBuilder.DropTable(
                name: "ProductKeys");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_ClientId",
                table: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_ProductKeyId",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "ProductKeyId",
                table: "Licenses");

            migrationBuilder.AddColumn<DateTime>(
                name: "GenerateDateTime",
                table: "Licenses",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProductKey",
                table: "Licenses",
                nullable: true);
        }
    }
}
