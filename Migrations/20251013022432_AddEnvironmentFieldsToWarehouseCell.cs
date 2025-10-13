using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FertilizerWarehouseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEnvironmentFieldsToWarehouseCell : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dimensions",
                table: "WarehouseCells",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ElectronicScale",
                table: "WarehouseCells",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Humidity",
                table: "WarehouseCells",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SensorStatus",
                table: "WarehouseCells",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Temperature",
                table: "WarehouseCells",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ventilation",
                table: "WarehouseCells",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 13, 2, 24, 28, 877, DateTimeKind.Utc).AddTicks(1117));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dimensions",
                table: "WarehouseCells");

            migrationBuilder.DropColumn(
                name: "ElectronicScale",
                table: "WarehouseCells");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "WarehouseCells");

            migrationBuilder.DropColumn(
                name: "SensorStatus",
                table: "WarehouseCells");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "WarehouseCells");

            migrationBuilder.DropColumn(
                name: "Ventilation",
                table: "WarehouseCells");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 8, 8, 34, 24, 260, DateTimeKind.Utc).AddTicks(6180));
        }
    }
}
