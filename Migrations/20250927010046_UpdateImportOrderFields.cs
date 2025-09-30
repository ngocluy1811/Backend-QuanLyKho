using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FertilizerWarehouseAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImportOrderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "ImportOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillOfLadingNumber",
                table: "ImportOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "ImportOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContainerNumber",
                table: "ImportOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "ImportOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverName",
                table: "ImportOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExportTax",
                table: "ImportOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ImportDate",
                table: "ImportOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "ImportOrders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptCode",
                table: "ImportOrders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalWeight",
                table: "ImportOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehiclePlateNumber",
                table: "ImportOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArrivalBatchNumber",
                table: "ImportOrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalDate",
                table: "ImportOrderDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContainerVehicleNumber",
                table: "ImportOrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 27, 1, 0, 44, 720, DateTimeKind.Utc).AddTicks(216));

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_CompanyId",
                table: "ImportOrders",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportOrders_Companies_CompanyId",
                table: "ImportOrders",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportOrders_Companies_CompanyId",
                table: "ImportOrders");

            migrationBuilder.DropIndex(
                name: "IX_ImportOrders_CompanyId",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "BillOfLadingNumber",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "ContainerNumber",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "DriverName",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "ExportTax",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "ImportDate",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "ReceiptCode",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "TotalWeight",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "VehiclePlateNumber",
                table: "ImportOrders");

            migrationBuilder.DropColumn(
                name: "ArrivalBatchNumber",
                table: "ImportOrderDetails");

            migrationBuilder.DropColumn(
                name: "ArrivalDate",
                table: "ImportOrderDetails");

            migrationBuilder.DropColumn(
                name: "ContainerVehicleNumber",
                table: "ImportOrderDetails");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 10, 5, 56, 797, DateTimeKind.Utc).AddTicks(2772));
        }
    }
}
