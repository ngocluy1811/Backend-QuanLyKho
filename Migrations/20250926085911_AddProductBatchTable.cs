using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FertilizerWarehouseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProductBatchTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductBatches_WarehousePositions_PositionId",
                table: "ProductBatches");

            migrationBuilder.DropIndex(
                name: "IX_ProductBatches_PositionId",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "ProductBatches");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ProductBatches",
                newName: "WarehousePositionId");

            migrationBuilder.AddColumn<int>(
                name: "ProductBatchId",
                table: "WarehouseCells",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProductBatches",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProductionDate",
                table: "ProductBatches",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "InitialQuantity",
                table: "ProductBatches",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "ProductBatches",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentQuantity",
                table: "ProductBatches",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "BatchNumber",
                table: "ProductBatches",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BatchName",
                table: "ProductBatches",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductBatches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ProductBatches",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductBatches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RemainingQuantity",
                table: "ProductBatches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "ProductBatches",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "ProductBatches",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 8, 59, 10, 75, DateTimeKind.Utc).AddTicks(3751));

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseCells_ProductBatchId",
                table: "WarehouseCells",
                column: "ProductBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_WarehousePositionId",
                table: "ProductBatches",
                column: "WarehousePositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductBatches_WarehousePositions_WarehousePositionId",
                table: "ProductBatches",
                column: "WarehousePositionId",
                principalTable: "WarehousePositions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseCells_ProductBatches_ProductBatchId",
                table: "WarehouseCells",
                column: "ProductBatchId",
                principalTable: "ProductBatches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductBatches_WarehousePositions_WarehousePositionId",
                table: "ProductBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseCells_ProductBatches_ProductBatchId",
                table: "WarehouseCells");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseCells_ProductBatchId",
                table: "WarehouseCells");

            migrationBuilder.DropIndex(
                name: "IX_ProductBatches_WarehousePositionId",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "ProductBatchId",
                table: "WarehouseCells");

            migrationBuilder.DropColumn(
                name: "BatchName",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "RemainingQuantity",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "ProductBatches");

            migrationBuilder.RenameColumn(
                name: "WarehousePositionId",
                table: "ProductBatches",
                newName: "UpdatedBy");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProductBatches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProductionDate",
                table: "ProductBatches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InitialQuantity",
                table: "ProductBatches",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "ProductBatches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentQuantity",
                table: "ProductBatches",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "BatchNumber",
                table: "ProductBatches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "ProductBatches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "ProductBatches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "ProductBatches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 8, 51, 30, 39, DateTimeKind.Utc).AddTicks(3486));

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_PositionId",
                table: "ProductBatches",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductBatches_WarehousePositions_PositionId",
                table: "ProductBatches",
                column: "PositionId",
                principalTable: "WarehousePositions",
                principalColumn: "Id");
        }
    }
}
