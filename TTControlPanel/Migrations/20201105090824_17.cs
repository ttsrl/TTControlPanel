using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Workings",
                newName: "StartDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Workings",
                newName: "EndDateTimeUtc");

            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                table: "Orders",
                newName: "DeliveryDateTimeUtc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDateTimeUtc",
                table: "Workings",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "EndDateTimeUtc",
                table: "Workings",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "DeliveryDateTimeUtc",
                table: "Orders",
                newName: "DeliveryDate");
        }
    }
}
