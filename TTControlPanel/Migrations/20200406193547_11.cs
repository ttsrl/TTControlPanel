using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationDate",
                table: "Users",
                newName: "TimestampDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "GenerateDateTime",
                table: "ProductKeys",
                newName: "TimestampDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "ReleaseDate",
                table: "Licenses",
                newName: "TimestampDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "ActivateDateTimeUtc",
                table: "Licenses",
                newName: "ActivationDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Hids",
                newName: "TimestampDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Clients",
                newName: "TimestampDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "ReleaseDate",
                table: "ApplicationsVersions",
                newName: "TimestampDateTimeUtc");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTimeUtc",
                table: "LastLogs",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDateTimeUtc",
                table: "ApplicationsVersions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseDateTimeUtc",
                table: "ApplicationsVersions");

            migrationBuilder.RenameColumn(
                name: "TimestampDateTimeUtc",
                table: "Users",
                newName: "RegistrationDate");

            migrationBuilder.RenameColumn(
                name: "TimestampDateTimeUtc",
                table: "ProductKeys",
                newName: "GenerateDateTime");

            migrationBuilder.RenameColumn(
                name: "TimestampDateTimeUtc",
                table: "Licenses",
                newName: "ReleaseDate");

            migrationBuilder.RenameColumn(
                name: "ActivationDateTimeUtc",
                table: "Licenses",
                newName: "ActivateDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "TimestampDateTimeUtc",
                table: "Hids",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "TimestampDateTimeUtc",
                table: "Clients",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "TimestampDateTimeUtc",
                table: "ApplicationsVersions",
                newName: "ReleaseDate");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTimeUtc",
                table: "LastLogs",
                nullable: true,
                oldClrType: typeof(DateTime));
        }
    }
}
