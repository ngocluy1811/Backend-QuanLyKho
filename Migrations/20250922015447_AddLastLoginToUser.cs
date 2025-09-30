using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FertilizerWarehouseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddLastLoginToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Users_SalesPersonId",
                table: "SalesOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Users_CreatedBy",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_WarehousePositions_PositionId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferDetails_StockTransfers_TransferId",
                table: "StockTransferDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferDetails_WarehousePositions_FromPositionId",
                table: "StockTransferDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferDetails_WarehousePositions_ToPositionId",
                table: "StockTransferDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Users_ApproverId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Users_RequesterId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComments_Users_UserId",
                table: "TaskComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssignedUserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssignerId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehousePositions_WarehouseZones_WarehouseZoneId",
                table: "WarehousePositions");

            migrationBuilder.DropIndex(
                name: "IX_WarehousePositions_WarehouseZoneId",
                table: "WarehousePositions");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignerId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_RequesterId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferDetails_FromPositionId",
                table: "StockTransferDetails");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferDetails_ToPositionId",
                table: "StockTransferDetails");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_CreatedBy",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_SalesPersonId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "WarehouseZones");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "WarehouseZones");

            migrationBuilder.DropColumn(
                name: "SpecialConditions",
                table: "WarehouseZones");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "WarehouseZones");

            migrationBuilder.DropColumn(
                name: "WarehouseZoneId",
                table: "WarehousePositions");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "FromPositionId",
                table: "StockTransferDetails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StockTransferDetails");

            migrationBuilder.DropColumn(
                name: "ToPositionId",
                table: "StockTransferDetails");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "SalesPersonId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "WarehouseZones",
                newName: "ZoneType");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "WarehouseZones",
                newName: "ZoneName");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Tasks",
                newName: "TaskType");

            migrationBuilder.RenameColumn(
                name: "Progress",
                table: "Tasks",
                newName: "ProgressPercentage");

            migrationBuilder.RenameColumn(
                name: "AssignerId",
                table: "Tasks",
                newName: "EstimatedHours");

            migrationBuilder.RenameColumn(
                name: "AssignedUserId",
                table: "Tasks",
                newName: "WarehouseId");

            migrationBuilder.RenameColumn(
                name: "AssignedBy",
                table: "Tasks",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_AssignedUserId",
                table: "Tasks",
                newName: "IX_Tasks_WarehouseId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TaskComments",
                newName: "CommentByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskComments_UserId",
                table: "TaskComments",
                newName: "IX_TaskComments_CommentByUserId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Suppliers",
                newName: "SupplierName");

            migrationBuilder.RenameColumn(
                name: "TotalItems",
                table: "StockTransfers",
                newName: "RequestedByUserId");

            migrationBuilder.RenameColumn(
                name: "ApproverId",
                table: "StockTransfers",
                newName: "ApprovedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransfers_ApproverId",
                table: "StockTransfers",
                newName: "IX_StockTransfers_ApprovedByUserId");

            migrationBuilder.RenameColumn(
                name: "TransferId",
                table: "StockTransferDetails",
                newName: "StockTransferId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransferDetails_TransferId",
                table: "StockTransferDetails",
                newName: "IX_StockTransferDetails_StockTransferId");

            migrationBuilder.RenameColumn(
                name: "ReferenceId",
                table: "StockMovements",
                newName: "WarehousePositionId");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "StockMovements",
                newName: "ToPositionId");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_PositionId",
                table: "StockMovements",
                newName: "IX_StockMovements_ToPositionId");

            migrationBuilder.RenameColumn(
                name: "SONumber",
                table: "SalesOrders",
                newName: "OrderNumber");

            migrationBuilder.RenameColumn(
                name: "PONumber",
                table: "PurchaseOrders",
                newName: "OrderNumber");

            migrationBuilder.RenameColumn(
                name: "SellingPrice",
                table: "Products",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "SKU",
                table: "Products",
                newName: "QualityStatus");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductCategories",
                newName: "CategoryName");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Notifications",
                newName: "NotificationType");

            migrationBuilder.RenameColumn(
                name: "ReferenceType",
                table: "Notifications",
                newName: "Link");

            migrationBuilder.RenameColumn(
                name: "ReferenceId",
                table: "Notifications",
                newName: "EntityId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Customers",
                newName: "CustomerType");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Customers",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Companies",
                newName: "CompanyName");

            migrationBuilder.AddColumn<int>(
                name: "CurrentCapacity",
                table: "WarehouseZones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxCapacity",
                table: "WarehouseZones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "WarehouseZones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZoneCode",
                table: "WarehouseZones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActualHours",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EntityId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CommentAt",
                table: "TaskComments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CommentBy",
                table: "TaskComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualDeliveryDate",
                table: "StockTransfers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "StockTransfers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "StockTransfers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDeliveryDate",
                table: "StockTransfers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "StockTransfers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "StockTransferDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "StockTransferDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FromPositionId",
                table: "StockMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "MovementDate",
                table: "StockMovements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "StockMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "StockMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "SalesOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "SalesOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDeliveryDate",
                table: "SalesOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "SalesOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "SalesOrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippedQuantity",
                table: "SalesOrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "PurchaseOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "PurchaseOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "PurchaseOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "PurchaseOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PurchaseOrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentStock",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TerminationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmploymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmergencyContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Productions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedTo = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Productions_Users_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseCells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    CellCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CellType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Row = table.Column<int>(type: "int", nullable: false),
                    Column = table.Column<int>(type: "int", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    CurrentAmount = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastMoved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseCells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseCells_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarehouseCells_WarehouseZones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "WarehouseZones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarehouseCells_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseClusters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ClusterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClusterType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CellIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseClusters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseClusters_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkHours = table.Column<TimeSpan>(type: "time", nullable: true),
                    OvertimeHours = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeaveRequests_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductionMachines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionId = table.Column<int>(type: "int", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Efficiency = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssignedOperator = table.Column<int>(type: "int", nullable: true),
                    LastMaintenance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextMaintenance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedOperatorUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionMachines_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductionMachines_Users_AssignedOperatorUserId",
                        column: x => x.AssignedOperatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 1, 54, 46, 689, DateTimeKind.Utc).AddTicks(3526));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastLogin", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 22, 1, 54, 46, 812, DateTimeKind.Utc).AddTicks(3916), null, "$2a$11$gu0fDc2QkhbUOPbFNRMtpeQLBhAOcLW0aAsNFwue5EUpPx3t.88GC" });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedByUserId",
                table: "Tasks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_RequestedByUserId",
                table: "StockTransfers",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_FromPositionId",
                table: "StockMovements",
                column: "FromPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_UserId",
                table: "StockMovements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_WarehouseId",
                table: "StockMovements",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_WarehousePositionId",
                table: "StockMovements",
                column: "WarehousePositionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CreatedBy",
                table: "SalesOrders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_WarehouseId",
                table: "SalesOrders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_WarehouseId",
                table: "PurchaseOrders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EmployeeId",
                table: "Attendances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_ApprovedByUserId",
                table: "LeaveRequests",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionMachines_AssignedOperatorUserId",
                table: "ProductionMachines",
                column: "AssignedOperatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionMachines_ProductionId",
                table: "ProductionMachines",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_Productions_AssignedUserId",
                table: "Productions",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Productions_ProductId",
                table: "Productions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseCells_ProductId",
                table: "WarehouseCells",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseCells_WarehouseId",
                table: "WarehouseCells",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseCells_ZoneId",
                table: "WarehouseCells",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseClusters_WarehouseId",
                table: "WarehouseClusters",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_SupplierId",
                table: "Products",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Warehouses_WarehouseId",
                table: "PurchaseOrders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Users_CreatedBy",
                table: "SalesOrders",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Warehouses_WarehouseId",
                table: "SalesOrders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Users_UserId",
                table: "StockMovements",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_WarehousePositions_FromPositionId",
                table: "StockMovements",
                column: "FromPositionId",
                principalTable: "WarehousePositions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_WarehousePositions_ToPositionId",
                table: "StockMovements",
                column: "ToPositionId",
                principalTable: "WarehousePositions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_WarehousePositions_WarehousePositionId",
                table: "StockMovements",
                column: "WarehousePositionId",
                principalTable: "WarehousePositions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Warehouses_WarehouseId",
                table: "StockMovements",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferDetails_StockTransfers_StockTransferId",
                table: "StockTransferDetails",
                column: "StockTransferId",
                principalTable: "StockTransfers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Users_ApprovedByUserId",
                table: "StockTransfers",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Users_RequestedByUserId",
                table: "StockTransfers",
                column: "RequestedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComments_Users_CommentByUserId",
                table: "TaskComments",
                column: "CommentByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_CreatedByUserId",
                table: "Tasks",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Warehouses_WarehouseId",
                table: "Tasks",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_SupplierId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Warehouses_WarehouseId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Users_CreatedBy",
                table: "SalesOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Warehouses_WarehouseId",
                table: "SalesOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Users_UserId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_WarehousePositions_FromPositionId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_WarehousePositions_ToPositionId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_WarehousePositions_WarehousePositionId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Warehouses_WarehouseId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferDetails_StockTransfers_StockTransferId",
                table: "StockTransferDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Users_ApprovedByUserId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Users_RequestedByUserId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComments_Users_CommentByUserId",
                table: "TaskComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_CreatedByUserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Warehouses_WarehouseId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "ProductionMachines");

            migrationBuilder.DropTable(
                name: "WarehouseCells");

            migrationBuilder.DropTable(
                name: "WarehouseClusters");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Productions");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedByUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_RequestedByUserId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_FromPositionId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_UserId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_WarehouseId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_WarehousePositionId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_CreatedBy",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_WarehouseId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_WarehouseId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_Products_SupplierId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CurrentCapacity",
                table: "WarehouseZones");

            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "WarehouseZones");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "WarehouseZones");

            migrationBuilder.DropColumn(
                name: "ZoneCode",
                table: "WarehouseZones");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ActualHours",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CommentAt",
                table: "TaskComments");

            migrationBuilder.DropColumn(
                name: "CommentBy",
                table: "TaskComments");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ActualDeliveryDate",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "ExpectedDeliveryDate",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "StockTransferDetails");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "StockTransferDetails");

            migrationBuilder.DropColumn(
                name: "FromPositionId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "MovementDate",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "RequestedDeliveryDate",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "SalesOrderDetails");

            migrationBuilder.DropColumn(
                name: "ShippedQuantity",
                table: "SalesOrderDetails");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "CurrentStock",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "ZoneType",
                table: "WarehouseZones",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ZoneName",
                table: "WarehouseZones",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                table: "Tasks",
                newName: "AssignedUserId");

            migrationBuilder.RenameColumn(
                name: "TaskType",
                table: "Tasks",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ProgressPercentage",
                table: "Tasks",
                newName: "Progress");

            migrationBuilder.RenameColumn(
                name: "EstimatedHours",
                table: "Tasks",
                newName: "AssignerId");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Tasks",
                newName: "AssignedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_WarehouseId",
                table: "Tasks",
                newName: "IX_Tasks_AssignedUserId");

            migrationBuilder.RenameColumn(
                name: "CommentByUserId",
                table: "TaskComments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskComments_CommentByUserId",
                table: "TaskComments",
                newName: "IX_TaskComments_UserId");

            migrationBuilder.RenameColumn(
                name: "SupplierName",
                table: "Suppliers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "RequestedByUserId",
                table: "StockTransfers",
                newName: "TotalItems");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUserId",
                table: "StockTransfers",
                newName: "ApproverId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransfers_ApprovedByUserId",
                table: "StockTransfers",
                newName: "IX_StockTransfers_ApproverId");

            migrationBuilder.RenameColumn(
                name: "StockTransferId",
                table: "StockTransferDetails",
                newName: "TransferId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransferDetails_StockTransferId",
                table: "StockTransferDetails",
                newName: "IX_StockTransferDetails_TransferId");

            migrationBuilder.RenameColumn(
                name: "WarehousePositionId",
                table: "StockMovements",
                newName: "ReferenceId");

            migrationBuilder.RenameColumn(
                name: "ToPositionId",
                table: "StockMovements",
                newName: "PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_ToPositionId",
                table: "StockMovements",
                newName: "IX_StockMovements_PositionId");

            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                table: "SalesOrders",
                newName: "SONumber");

            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                table: "PurchaseOrders",
                newName: "PONumber");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Products",
                newName: "SellingPrice");

            migrationBuilder.RenameColumn(
                name: "QualityStatus",
                table: "Products",
                newName: "SKU");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CategoryName",
                table: "ProductCategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "NotificationType",
                table: "Notifications",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Link",
                table: "Notifications",
                newName: "ReferenceType");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "Notifications",
                newName: "ReferenceId");

            migrationBuilder.RenameColumn(
                name: "CustomerType",
                table: "Customers",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "Customers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "Companies",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WarehouseZones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Humidity",
                table: "WarehouseZones",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialConditions",
                table: "WarehouseZones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature",
                table: "WarehouseZones",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseZoneId",
                table: "WarehousePositions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RequesterId",
                table: "StockTransfers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FromPositionId",
                table: "StockTransferDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "StockTransferDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ToPositionId",
                table: "StockTransferDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "StockMovements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SalesPersonId",
                table: "SalesOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 17, 10, 29, 15, 275, DateTimeKind.Utc).AddTicks(2333));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 17, 10, 29, 15, 820, DateTimeKind.Utc).AddTicks(4729), "$2a$11$IGrx1qDYtjIYc0ljsbuEV.pqpv7LRJQXbYIg3C.S47/UQSgzmyime" });

            migrationBuilder.CreateIndex(
                name: "IX_WarehousePositions_WarehouseZoneId",
                table: "WarehousePositions",
                column: "WarehouseZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignerId",
                table: "Tasks",
                column: "AssignerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_RequesterId",
                table: "StockTransfers",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferDetails_FromPositionId",
                table: "StockTransferDetails",
                column: "FromPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferDetails_ToPositionId",
                table: "StockTransferDetails",
                column: "ToPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_CreatedBy",
                table: "StockMovements",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_SalesPersonId",
                table: "SalesOrders",
                column: "SalesPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Users_SalesPersonId",
                table: "SalesOrders",
                column: "SalesPersonId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Users_CreatedBy",
                table: "StockMovements",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_WarehousePositions_PositionId",
                table: "StockMovements",
                column: "PositionId",
                principalTable: "WarehousePositions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferDetails_StockTransfers_TransferId",
                table: "StockTransferDetails",
                column: "TransferId",
                principalTable: "StockTransfers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferDetails_WarehousePositions_FromPositionId",
                table: "StockTransferDetails",
                column: "FromPositionId",
                principalTable: "WarehousePositions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferDetails_WarehousePositions_ToPositionId",
                table: "StockTransferDetails",
                column: "ToPositionId",
                principalTable: "WarehousePositions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Users_ApproverId",
                table: "StockTransfers",
                column: "ApproverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Users_RequesterId",
                table: "StockTransfers",
                column: "RequesterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComments_Users_UserId",
                table: "TaskComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssignedUserId",
                table: "Tasks",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssignerId",
                table: "Tasks",
                column: "AssignerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehousePositions_WarehouseZones_WarehouseZoneId",
                table: "WarehousePositions",
                column: "WarehouseZoneId",
                principalTable: "WarehouseZones",
                principalColumn: "Id");
        }
    }
}
