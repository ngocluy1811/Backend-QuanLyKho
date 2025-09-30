using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FertilizerWarehouseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerFieldsToImportOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "ImportOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ImportOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "ImportOrders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone",
                table: "ImportOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExportReason",
                table: "ImportOrders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 11, 44, 26, 379, DateTimeKind.Utc).AddTicks(7429));

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_CustomerId",
                table: "ImportOrders",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportOrders_Suppliers_CustomerId",
                table: "ImportOrders",
                column: "CustomerId",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportOrders_Suppliers_CustomerId",
                table: "ImportOrders");

            migrationBuilder.DropIndex(
                name: "IX_ImportOrders_CustomerId",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "ExportReason",
                table: "ImportOrders");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 27, 15, 59, 0, 951, DateTimeKind.Utc).AddTicks(368));
        }
    }
}
