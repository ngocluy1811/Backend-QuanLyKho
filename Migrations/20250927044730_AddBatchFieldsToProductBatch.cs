using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FertilizerWarehouseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBatchFieldsToProductBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayVe",
                table: "ProductBatches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoDotVe",
                table: "ProductBatches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoXeContainerTungDot",
                table: "ProductBatches",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 27, 4, 47, 29, 101, DateTimeKind.Utc).AddTicks(510));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayVe",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "SoDotVe",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "SoXeContainerTungDot",
                table: "ProductBatches");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 27, 1, 41, 6, 720, DateTimeKind.Utc).AddTicks(9059));
        }
    }
}
