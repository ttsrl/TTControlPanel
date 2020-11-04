using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TTControlPanel.Migrations
{
    public partial class _14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Orders_OrderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Invoice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Invoces",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: true),
                    FinalClientId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    TimestampDateTimeUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workings_Clients_FinalClientId",
                        column: x => x.FinalClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Workings_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkingItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    OperatorId = table.Column<int>(nullable: true),
                    TimestampDateTimeUtc = table.Column<DateTime>(nullable: false),
                    WorkingId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkingItem_Users_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkingItem_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkingItem_Workings_WorkingId",
                        column: x => x.WorkingId,
                        principalTable: "Workings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_InvoiceId",
                table: "Orders",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingItem_OperatorId",
                table: "WorkingItem",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingItem_ProductId",
                table: "WorkingItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingItem_WorkingId",
                table: "WorkingItem",
                column: "WorkingId");

            migrationBuilder.CreateIndex(
                name: "IX_Workings_FinalClientId",
                table: "Workings",
                column: "FinalClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Workings_OrderId",
                table: "Workings",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Invoces_InvoiceId",
                table: "Orders",
                column: "InvoiceId",
                principalTable: "Invoces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Invoces_InvoiceId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Invoces");

            migrationBuilder.DropTable(
                name: "WorkingItem");

            migrationBuilder.DropTable(
                name: "Workings");

            migrationBuilder.DropIndex(
                name: "IX_Orders_InvoiceId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Invoice",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrderId",
                table: "Products",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Orders_OrderId",
                table: "Products",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
