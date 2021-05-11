using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizationLogs");

            migrationBuilder.DropTable(
                name: "SshKeys");

            migrationBuilder.DropTable(
                name: "TeamRepositoryRoles");

            migrationBuilder.DropTable(
                name: "UserTeamRoles");

            migrationBuilder.DropTable(
                name: "Teams");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorizationLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssueIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AuthorizationLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SshKeys",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fingerprint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImportData = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KeyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUse = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublicKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SshKeys", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SshKeys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TeamRepositoryRoles",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllowRead = table.Column<bool>(type: "bit", nullable: false),
                    AllowWrite = table.Column<bool>(type: "bit", nullable: false),
                    RepositoryID = table.Column<long>(type: "bigint", nullable: true),
                    TeamID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamRepositoryRoles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TeamRepositoryRoles_Repositories_RepositoryID",
                        column: x => x.RepositoryID,
                        principalTable: "Repositories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamRepositoryRoles_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTeamRoles",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAdministrator = table.Column<bool>(type: "bit", nullable: false),
                    TeamID = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTeamRoles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserTeamRoles_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTeamRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationLogs_UserId",
                table: "AuthorizationLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SshKeys_UserId",
                table: "SshKeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamRepositoryRoles_RepositoryID",
                table: "TeamRepositoryRoles",
                column: "RepositoryID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamRepositoryRoles_TeamID",
                table: "TeamRepositoryRoles",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamRoles_TeamID",
                table: "UserTeamRoles",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamRoles_UserId",
                table: "UserTeamRoles",
                column: "UserId");
        }
    }
}
