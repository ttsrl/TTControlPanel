using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workings_Orders_OrderId",
                table: "Workings");

            migrationBuilder.DropIndex(
                name: "IX_Workings_OrderId",
                table: "Workings");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Workings");

            migrationBuilder.AddColumn<int>(
                name: "WorkingId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WorkingId",
                table: "Orders",
                column: "WorkingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Workings_WorkingId",
                table: "Orders",
                column: "WorkingId",
                principalTable: "Workings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Workings_WorkingId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_WorkingId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WorkingId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Workings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workings_OrderId",
                table: "Workings",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workings_Orders_OrderId",
                table: "Workings",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
