using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FertilizerWarehouseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDetailsToWarehouseCell : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "WarehouseCells",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionDate",
                table: "WarehouseCells",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "WarehouseCells",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 6, 20, 51, 84, DateTimeKind.Utc).AddTicks(1616));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "WarehouseCells");

            migrationBuilder.DropColumn(
                name: "ProductionDate",
                table: "WarehouseCells");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "WarehouseCells");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 6, 7, 27, 516, DateTimeKind.Utc).AddTicks(1314));
        }
    }
}
