using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorizationLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    IssueDate = table.Column<DateTime>(nullable: false),
                    Expires = table.Column<DateTime>(nullable: false),
                    IssueIp = table.Column<string>(nullable: true),
                    LastIp = table.Column<string>(nullable: true),
                    IsValid = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
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
                name: "Repositories",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DefaultBranch = table.Column<string>(nullable: true),
                    NumIssues = table.Column<int>(nullable: false),
                    NumOpenIssues = table.Column<int>(nullable: false),
                    NumPulls = table.Column<int>(nullable: false),
                    NumOpenPulls = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    IsMirror = table.Column<bool>(nullable: false),
                    Size = table.Column<long>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SshKeys",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyType = table.Column<string>(nullable: true),
                    Fingerprint = table.Column<string>(nullable: true),
                    PublicKey = table.Column<string>(nullable: true),
                    ImportData = table.Column<DateTime>(nullable: false),
                    LastUse = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
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
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TeamRepositoryRoles",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllowRead = table.Column<bool>(nullable: false),
                    AllowWrite = table.Column<bool>(nullable: false),
                    RepositoryID = table.Column<long>(nullable: true),
                    TeamID = table.Column<long>(nullable: true)
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
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAdministrator = table.Column<bool>(nullable: false),
                    TeamID = table.Column<long>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Repositories");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
