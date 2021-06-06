using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeepAliveRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Response = table.Column<int>(nullable: false),
                    DurationRequest = table.Column<int>(nullable: false),
                    Inizialize = table.Column<bool>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeepAliveRequests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeepAliveRequests");
        }
    }
}
