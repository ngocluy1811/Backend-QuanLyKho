using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FertilizerWarehouseAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Barcodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    BarcodeType = table.Column<string>(type: "text", nullable: false),
                    BarcodeValue = table.Column<string>(type: "text", nullable: false),
                    BarcodeImage = table.Column<string>(type: "text", nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PrintedCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barcodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProcessedBy = table.Column<int>(type: "integer", nullable: true),
                    AdminNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PasswordResetAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlertRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    RuleName = table.Column<string>(type: "text", nullable: false),
                    AlertType = table.Column<string>(type: "text", nullable: false),
                    Condition = table.Column<string>(type: "text", nullable: false),
                    Threshold = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    NotificationChannels = table.Column<string>(type: "text", nullable: false),
                    Recipients = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertRules_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CategoryName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ParentCategoryId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    SupplierName = table.Column<string>(type: "text", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    ContactPerson = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    TaxCode = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    BankAccount = table.Column<string>(type: "text", nullable: true),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    PaymentTerms = table.Column<string>(type: "text", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suppliers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    LicensePlate = table.Column<string>(type: "text", nullable: false),
                    Capacity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DriverName = table.Column<string>(type: "text", nullable: true),
                    DriverPhone = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LastMaintenance = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextMaintenance = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PermissionKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PermissionName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MinStockLevel = table.Column<int>(type: "integer", nullable: false),
                    MaxStockLevel = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    ProductionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BatchNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductCompositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Component = table.Column<string>(type: "text", nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCompositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCompositions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TriggerCondition = table.Column<string>(type: "text", nullable: false),
                    AffectedEntity = table.Column<string>(type: "text", nullable: true),
                    AffectedEntityId = table.Column<int>(type: "integer", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResolvedBy = table.Column<int>(type: "integer", nullable: true),
                    ResolverId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OvertimeHours = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    OvertimeStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OvertimeEndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    IsOvertimeRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WorkHours = table.Column<TimeSpan>(type: "interval", nullable: true),
                    OvertimeHours = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: true),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    UserAgent = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ManagerId = table.Column<int>(type: "integer", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeCode = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    HireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TerminationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmploymentStatus = table.Column<string>(type: "text", nullable: false),
                    Salary = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmergencyContact = table.Column<string>(type: "text", nullable: true),
                    EmergencyPhone = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LockoutEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PasswordExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MustChangePassword = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorSecret = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EntityTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TagId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    AssignedBy = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EntityTags_Users_AssignedBy",
                        column: x => x.AssignedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: false),
                    ReferenceType = table.Column<string>(type: "text", nullable: true),
                    ReferenceId = table.Column<int>(type: "integer", nullable: true),
                    UploadedBy = table.Column<int>(type: "integer", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploaderId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileAttachments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAttachments_Users_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    LeaveType = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalDays = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovalNotes = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequest_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeaveRequest_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalDays = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comments = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeaveRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LoginHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DeviceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OperatingSystem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Browser = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DeviceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CountryCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Timezone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsSuccessful = table.Column<bool>(type: "boolean", nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AdditionalInfo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsSuspicious = table.Column<bool>(type: "boolean", nullable: false),
                    SuspiciousReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SessionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SessionExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LoginMethod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActiveSession = table.Column<bool>(type: "boolean", nullable: false),
                    IsFromNewDevice = table.Column<bool>(type: "boolean", nullable: false),
                    IsFromNewLocation = table.Column<bool>(type: "boolean", nullable: false),
                    IsFromNewIp = table.Column<bool>(type: "boolean", nullable: false),
                    IsOffHoursLogin = table.Column<bool>(type: "boolean", nullable: false),
                    IsRapidLogin = table.Column<bool>(type: "boolean", nullable: false),
                    RiskScore = table.Column<int>(type: "integer", nullable: false),
                    RiskLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    NotificationType = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<int>(type: "integer", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Productions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductionOrderNumber = table.Column<string>(type: "text", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PlannedEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    AssignedTo = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignedUserId = table.Column<int>(type: "integer", nullable: true)
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
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Period = table.Column<string>(type: "text", nullable: false),
                    SubmittedBy = table.Column<int>(type: "integer", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    SubmitterId = table.Column<int>(type: "integer", nullable: false),
                    ApproverId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Users_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SecurityAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AlertType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AlertTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DeviceInfo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AdditionalData = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ActionTaken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RequiresUserAction = table.Column<bool>(type: "boolean", nullable: false),
                    UserActionRequired = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UserActionDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsNotified = table.Column<bool>(type: "boolean", nullable: false),
                    NotificationSentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NotificationMethod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecurityAlerts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SecurityEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Details = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EventTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecurityEvents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    AdjustmentNumber = table.Column<string>(type: "text", nullable: false),
                    AdjustmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdjustmentType = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    TotalValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    EvidencePhoto = table.Column<string>(type: "text", nullable: true),
                    ApproverId = table.Column<int>(type: "integer", nullable: true),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false),
                    UpdaterId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemSettings_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SystemSettings_Users_UpdaterId",
                        column: x => x.UpdaterId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrustedDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DeviceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OperatingSystem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Browser = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CountryCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FirstSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LoginCount = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false),
                    BlockReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeviceFingerprint = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TrustLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RiskScore = table.Column<int>(type: "integer", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrustedDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrustedDevices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PermissionKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PermissionName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PermissionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_AssignedBy",
                        column: x => x.AssignedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    UserAgent = table.Column<string>(type: "text", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LogoutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    ManagerId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Warehouses_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductionMachines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductionId = table.Column<int>(type: "integer", nullable: false),
                    MachineName = table.Column<string>(type: "text", nullable: false),
                    MachineType = table.Column<string>(type: "text", nullable: false),
                    MachineCode = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Efficiency = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AssignedOperator = table.Column<int>(type: "integer", nullable: true),
                    LastMaintenance = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextMaintenance = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignedOperatorUserId = table.Column<int>(type: "integer", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "ImportOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    CustomerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CustomerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CustomerAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ExportReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    TotalValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ContainerNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    VehiclePlateNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DriverName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TotalWeight = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    BatchNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillOfLadingNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ExportTax = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ReceiptCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ImportDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VoucherCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TaxRate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    DiscountPercent = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImportOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImportOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImportOrders_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CheckNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CheckDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryChecks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: false),
                    OrderNumber = table.Column<string>(type: "text", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SalesOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    OrderNumber = table.Column<string>(type: "text", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequestedDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrders_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrders_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockTransfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    FromWarehouseId = table.Column<int>(type: "integer", nullable: false),
                    ToWarehouseId = table.Column<int>(type: "integer", nullable: false),
                    TransferNumber = table.Column<string>(type: "text", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequestedBy = table.Column<int>(type: "integer", nullable: false),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    RequestedByUserId = table.Column<int>(type: "integer", nullable: false),
                    ApprovedByUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransfers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTransfers_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTransfers_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTransfers_Warehouses_FromWarehouseId",
                        column: x => x.FromWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTransfers_Warehouses_ToWarehouseId",
                        column: x => x.ToWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TaskType = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    AssignedTo = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProgressPercentage = table.Column<int>(type: "integer", nullable: false),
                    EstimatedHours = table.Column<int>(type: "integer", nullable: false),
                    ActualHours = table.Column<int>(type: "integer", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: true),
                    EntityType = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<int>(type: "integer", nullable: true),
                    AssignedToUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserWarehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWarehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWarehouses_Users_AssignedBy",
                        column: x => x.AssignedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWarehouses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWarehouses_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseClusters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    ClusterName = table.Column<string>(type: "text", nullable: false),
                    ClusterType = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    CellIds = table.Column<List<int>>(type: "integer[]", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "WarehousePositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Zone = table.Column<string>(type: "text", nullable: false),
                    Row = table.Column<int>(type: "integer", nullable: false),
                    Column = table.Column<int>(type: "integer", nullable: false),
                    MaxCapacity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CurrentCapacity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedStaff = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehousePositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehousePositions_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    ZoneName = table.Column<string>(type: "text", nullable: false),
                    ZoneCode = table.Column<string>(type: "text", nullable: false),
                    ZoneType = table.Column<string>(type: "text", nullable: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false),
                    CurrentCapacity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseZones_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PurchaseOrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    CommentBy = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CommentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AttachmentPath = table.Column<string>(type: "text", nullable: true),
                    CommentByUserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskComments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskComments_Users_CommentByUserId",
                        column: x => x.CommentByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BatchNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BatchName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    RemainingQuantity = table.Column<int>(type: "integer", nullable: false),
                    InitialQuantity = table.Column<int>(type: "integer", nullable: false),
                    CurrentQuantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    QualityStatus = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    WarehousePositionId = table.Column<int>(type: "integer", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    TotalValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NgayVe = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SoDotVe = table.Column<int>(type: "integer", nullable: true),
                    SoXeContainerTungDot = table.Column<int>(type: "integer", nullable: true),
                    NgayVeDetails = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBatches_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductBatches_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductBatches_WarehousePositions_WarehousePositionId",
                        column: x => x.WarehousePositionId,
                        principalTable: "WarehousePositions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockTakes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    StockTakeNumber = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: true),
                    ZoneId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SupervisorId = table.Column<int>(type: "integer", nullable: false),
                    TotalItems = table.Column<int>(type: "integer", nullable: false),
                    VarianceValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTakes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTakes_Users_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTakes_WarehouseZones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "WarehouseZones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTakes_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    BatchId = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ShippedQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderDetails_ProductBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "ProductBatches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrderDetails_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustmentDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdjustmentId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    BatchId = table.Column<int>(type: "integer", nullable: false),
                    PositionId = table.Column<int>(type: "integer", nullable: true),
                    SystemQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DifferenceQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustmentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockAdjustmentDetails_ProductBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "ProductBatches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockAdjustmentDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockAdjustmentDetails_StockAdjustments_AdjustmentId",
                        column: x => x.AdjustmentId,
                        principalTable: "StockAdjustments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockAdjustmentDetails_WarehousePositions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "WarehousePositions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    BatchId = table.Column<int>(type: "integer", nullable: true),
                    PositionId = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    LastMovementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockItems_ProductBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "ProductBatches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockItems_WarehousePositions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "WarehousePositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockItems_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    BatchId = table.Column<int>(type: "integer", nullable: false),
                    FromPositionId = table.Column<int>(type: "integer", nullable: false),
                    ToPositionId = table.Column<int>(type: "integer", nullable: true),
                    MovementType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    MovementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    WarehousePositionId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_ProductBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "ProductBatches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_WarehousePositions_FromPositionId",
                        column: x => x.FromPositionId,
                        principalTable: "WarehousePositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_WarehousePositions_ToPositionId",
                        column: x => x.ToPositionId,
                        principalTable: "WarehousePositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_WarehousePositions_WarehousePositionId",
                        column: x => x.WarehousePositionId,
                        principalTable: "WarehousePositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockTransferDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StockTransferId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    BatchId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TransferredQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransferDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransferDetails_ProductBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "ProductBatches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTransferDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTransferDetails_StockTransfers_StockTransferId",
                        column: x => x.StockTransferId,
                        principalTable: "StockTransfers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseCells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    ZoneId = table.Column<int>(type: "integer", nullable: false),
                    CellCode = table.Column<string>(type: "text", nullable: false),
                    CellType = table.Column<string>(type: "text", nullable: false),
                    Row = table.Column<int>(type: "integer", nullable: false),
                    Column = table.Column<int>(type: "integer", nullable: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false),
                    CurrentAmount = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    BatchNumber = table.Column<string>(type: "text", nullable: true),
                    ProductionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Supplier = table.Column<string>(type: "text", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ProductBatchId = table.Column<int>(type: "integer", nullable: true),
                    LastMoved = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ClusterName = table.Column<string>(type: "text", nullable: true),
                    AssignedStaff = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseCells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseCells_ProductBatches_ProductBatchId",
                        column: x => x.ProductBatchId,
                        principalTable: "ProductBatches",
                        principalColumn: "Id");
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
                name: "StockTakeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StockTakeId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    BatchId = table.Column<int>(type: "integer", nullable: false),
                    PositionId = table.Column<int>(type: "integer", nullable: true),
                    SystemQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CountedQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    VarianceQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    VarianceValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CountedBy = table.Column<int>(type: "integer", nullable: false),
                    CountedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CounterId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTakeDetails_ProductBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "ProductBatches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTakeDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTakeDetails_StockTakes_StockTakeId",
                        column: x => x.StockTakeId,
                        principalTable: "StockTakes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTakeDetails_Users_CounterId",
                        column: x => x.CounterId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTakeDetails_WarehousePositions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "WarehousePositions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ImportOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImportOrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductBatchId = table.Column<int>(type: "integer", nullable: true),
                    WarehouseCellId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ReceivedQuantity = table.Column<int>(type: "integer", nullable: true),
                    RemainingQuantity = table.Column<int>(type: "integer", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    BatchNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProductionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Supplier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArrivalBatchNumber = table.Column<int>(type: "integer", nullable: true),
                    ContainerVehicleNumber = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportOrderDetails_ImportOrders_ImportOrderId",
                        column: x => x.ImportOrderId,
                        principalTable: "ImportOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImportOrderDetails_ProductBatches_ProductBatchId",
                        column: x => x.ProductBatchId,
                        principalTable: "ProductBatches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImportOrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImportOrderDetails_WarehouseCells_WarehouseCellId",
                        column: x => x.WarehouseCellId,
                        principalTable: "WarehouseCells",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryCheckItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryCheckId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    WarehouseCellId = table.Column<int>(type: "integer", nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SystemQuantity = table.Column<int>(type: "integer", nullable: false),
                    ActualQuantity = table.Column<int>(type: "integer", nullable: false),
                    Difference = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCheckItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCheckItems_InventoryChecks_InventoryCheckId",
                        column: x => x.InventoryCheckId,
                        principalTable: "InventoryChecks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryCheckItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryCheckItems_WarehouseCells_WarehouseCellId",
                        column: x => x.WarehouseCellId,
                        principalTable: "WarehouseCells",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MaintenanceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AssignedTo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstimatedDuration = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    WarehouseCellId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequests_WarehouseCells_WarehouseCellId",
                        column: x => x.WarehouseCellId,
                        principalTable: "WarehouseCells",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaintenanceRequests_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    CellId = table.Column<int>(type: "integer", nullable: true),
                    ActivityType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    BatchNumber = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    FromLocation = table.Column<string>(type: "text", nullable: true),
                    ToLocation = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarehouseActivities_WarehouseCells_CellId",
                        column: x => x.CellId,
                        principalTable: "WarehouseCells",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarehouseActivities_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseCellProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseCellId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    RemainingQuantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Supplier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArrivalBatchNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContainerVehicleNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseCellProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseCellProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarehouseCellProducts_WarehouseCells_WarehouseCellId",
                        column: x => x.WarehouseCellId,
                        principalTable: "WarehouseCells",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockTakeAdjustments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StockTakeDetailId = table.Column<int>(type: "integer", nullable: false),
                    AdjustmentId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakeAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTakeAdjustments_StockAdjustments_AdjustmentId",
                        column: x => x.AdjustmentId,
                        principalTable: "StockAdjustments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTakeAdjustments_StockTakeDetails_StockTakeDetailId",
                        column: x => x.StockTakeDetailId,
                        principalTable: "StockTakeDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaintenanceRequestId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Progress = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceHistories_MaintenanceRequests_MaintenanceRequest~",
                        column: x => x.MaintenanceRequestId,
                        principalTable: "MaintenanceRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Address", "Code", "CompanyName", "CreatedAt", "Department", "Email", "IsActive", "Phone", "Status", "TaxCode", "UpdatedAt" },
                values: new object[] { 1, "123 Main Street", "FWC", "Fertilizer Warehouse Company", new DateTime(2025, 10, 8, 8, 34, 24, 260, DateTimeKind.Utc).AddTicks(6180), null, "admin@fwc.com", true, "0123456789", "Active", "123456789", null });

            migrationBuilder.CreateIndex(
                name: "IX_AlertRules_CompanyId",
                table: "AlertRules",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_CompanyId",
                table: "Alerts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ResolverId",
                table: "Alerts",
                column: "ResolverId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_UserId",
                table: "AttendanceRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EmployeeId",
                table: "Attendances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CompanyId",
                table: "AuditLogs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_SessionId",
                table: "AuditLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CompanyId",
                table: "Customers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId",
                table: "Departments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityTags_AssignedBy",
                table: "EntityTags",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityTags_TagId",
                table: "EntityTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_CompanyId",
                table: "FileAttachments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_UploaderId",
                table: "FileAttachments",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrderDetails_ImportOrderId",
                table: "ImportOrderDetails",
                column: "ImportOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrderDetails_ProductBatchId",
                table: "ImportOrderDetails",
                column: "ProductBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrderDetails_ProductId",
                table: "ImportOrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrderDetails_WarehouseCellId",
                table: "ImportOrderDetails",
                column: "WarehouseCellId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_CompanyId",
                table: "ImportOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_CustomerId",
                table: "ImportOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_SupplierId",
                table: "ImportOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_WarehouseId",
                table: "ImportOrders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckItems_InventoryCheckId",
                table: "InventoryCheckItems",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckItems_ProductId",
                table: "InventoryCheckItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckItems_WarehouseCellId",
                table: "InventoryCheckItems",
                column: "WarehouseCellId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_WarehouseId",
                table: "InventoryChecks",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_ApprovedByUserId",
                table: "LeaveRequest",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_EmployeeId",
                table: "LeaveRequest",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_ApprovedBy",
                table: "LeaveRequests",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_UserId",
                table: "LeaveRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginHistories_UserId",
                table: "LoginHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_MaintenanceRequestId",
                table: "MaintenanceHistories",
                column: "MaintenanceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_WarehouseCellId",
                table: "MaintenanceRequests",
                column: "WarehouseCellId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_WarehouseId",
                table: "MaintenanceRequests",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CompanyId",
                table: "Notifications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_ProductId",
                table: "ProductBatches",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_SupplierId",
                table: "ProductBatches",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_WarehousePositionId",
                table: "ProductBatches",
                column: "WarehousePositionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CompanyId",
                table: "ProductCategories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentCategoryId",
                table: "ProductCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCompositions_ProductId",
                table: "ProductCompositions",
                column: "ProductId");

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
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CompanyId",
                table: "Products",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_ProductId",
                table: "PurchaseOrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_PurchaseOrderId",
                table: "PurchaseOrderDetails",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_ApprovedBy",
                table: "PurchaseOrders",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CompanyId",
                table: "PurchaseOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CreatedBy",
                table: "PurchaseOrders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SupplierId",
                table: "PurchaseOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_WarehouseId",
                table: "PurchaseOrders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ApproverId",
                table: "Reports",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CompanyId",
                table: "Reports",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_DepartmentId",
                table: "Reports",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SubmitterId",
                table: "Reports",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_Role_PermissionKey",
                table: "RolePermissions",
                columns: new[] { "Role", "PermissionKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderDetails_BatchId",
                table: "SalesOrderDetails",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderDetails_ProductId",
                table: "SalesOrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderDetails_SalesOrderId",
                table: "SalesOrderDetails",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CompanyId",
                table: "SalesOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CreatedBy",
                table: "SalesOrders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CustomerId",
                table: "SalesOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_WarehouseId",
                table: "SalesOrders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAlerts_UserId",
                table: "SecurityAlerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_UserId",
                table: "SecurityEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentDetails_AdjustmentId",
                table: "StockAdjustmentDetails",
                column: "AdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentDetails_BatchId",
                table: "StockAdjustmentDetails",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentDetails_PositionId",
                table: "StockAdjustmentDetails",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentDetails_ProductId",
                table: "StockAdjustmentDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_ApproverId",
                table: "StockAdjustments",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_CompanyId",
                table: "StockAdjustments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_CreatorId",
                table: "StockAdjustments",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_BatchId",
                table: "StockItems",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_PositionId",
                table: "StockItems",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_ProductId",
                table: "StockItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_WarehouseId_ProductId_BatchId_PositionId",
                table: "StockItems",
                columns: new[] { "WarehouseId", "ProductId", "BatchId", "PositionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_BatchId",
                table: "StockMovements",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_CompanyId",
                table: "StockMovements",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_FromPositionId",
                table: "StockMovements",
                column: "FromPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductId",
                table: "StockMovements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ToPositionId",
                table: "StockMovements",
                column: "ToPositionId");

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
                name: "IX_StockTakeAdjustments_AdjustmentId",
                table: "StockTakeAdjustments",
                column: "AdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeAdjustments_StockTakeDetailId",
                table: "StockTakeAdjustments",
                column: "StockTakeDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeDetails_BatchId",
                table: "StockTakeDetails",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeDetails_CounterId",
                table: "StockTakeDetails",
                column: "CounterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeDetails_PositionId",
                table: "StockTakeDetails",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeDetails_ProductId",
                table: "StockTakeDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeDetails_StockTakeId",
                table: "StockTakeDetails",
                column: "StockTakeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakes_CompanyId",
                table: "StockTakes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakes_SupervisorId",
                table: "StockTakes",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakes_WarehouseId",
                table: "StockTakes",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakes_ZoneId",
                table: "StockTakes",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferDetails_BatchId",
                table: "StockTransferDetails",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferDetails_ProductId",
                table: "StockTransferDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferDetails_StockTransferId",
                table: "StockTransferDetails",
                column: "StockTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ApprovedByUserId",
                table: "StockTransfers",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_CompanyId",
                table: "StockTransfers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_FromWarehouseId",
                table: "StockTransfers",
                column: "FromWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_RequestedByUserId",
                table: "StockTransfers",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ToWarehouseId",
                table: "StockTransfers",
                column: "ToWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_CompanyId",
                table: "Suppliers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_CompanyId",
                table: "SystemSettings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_UpdaterId",
                table: "SystemSettings",
                column: "UpdaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CompanyId",
                table: "Tags",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_CommentByUserId",
                table: "TaskComments",
                column: "CommentByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_TaskId",
                table: "TaskComments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CompanyId",
                table: "Tasks",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedByUserId",
                table: "Tasks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DepartmentId",
                table: "Tasks",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_WarehouseId",
                table: "Tasks",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_TrustedDevices_UserId",
                table: "TrustedDevices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId_PermissionKey",
                table: "UserPermissions",
                columns: new[] { "UserId", "PermissionKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AssignedBy",
                table: "UserRoles",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_RoleId",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWarehouses_AssignedBy",
                table: "UserWarehouses",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserWarehouses_UserId_WarehouseId",
                table: "UserWarehouses",
                columns: new[] { "UserId", "WarehouseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWarehouses_WarehouseId",
                table: "UserWarehouses",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CompanyId",
                table: "Vehicles",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseActivities_CellId",
                table: "WarehouseActivities",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseActivities_UserId",
                table: "WarehouseActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseActivities_WarehouseId",
                table: "WarehouseActivities",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseCellProducts_ProductId",
                table: "WarehouseCellProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseCellProducts_WarehouseCellId",
                table: "WarehouseCellProducts",
                column: "WarehouseCellId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseCells_ProductBatchId",
                table: "WarehouseCells",
                column: "ProductBatchId");

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

            migrationBuilder.CreateIndex(
                name: "IX_WarehousePositions_WarehouseId",
                table: "WarehousePositions",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_CompanyId",
                table: "Warehouses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_ManagerId",
                table: "Warehouses",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseZones_WarehouseId",
                table: "WarehouseZones",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Users_ResolverId",
                table: "Alerts",
                column: "ResolverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Users_UserId",
                table: "AttendanceRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Employees_EmployeeId",
                table: "Attendances",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_UserSessions_SessionId",
                table: "AuditLogs",
                column: "SessionId",
                principalTable: "UserSessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Users_ManagerId",
                table: "Departments",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Companies_CompanyId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_ManagerId",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "AlertRules");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Barcodes");

            migrationBuilder.DropTable(
                name: "EntityTags");

            migrationBuilder.DropTable(
                name: "FileAttachments");

            migrationBuilder.DropTable(
                name: "ImportOrderDetails");

            migrationBuilder.DropTable(
                name: "InventoryCheckItems");

            migrationBuilder.DropTable(
                name: "LeaveRequest");

            migrationBuilder.DropTable(
                name: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "LoginHistories");

            migrationBuilder.DropTable(
                name: "MaintenanceHistories");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PasswordResetRequests");

            migrationBuilder.DropTable(
                name: "ProductCompositions");

            migrationBuilder.DropTable(
                name: "ProductionMachines");

            migrationBuilder.DropTable(
                name: "PurchaseOrderDetails");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SalesOrderDetails");

            migrationBuilder.DropTable(
                name: "SecurityAlerts");

            migrationBuilder.DropTable(
                name: "SecurityEvents");

            migrationBuilder.DropTable(
                name: "StockAdjustmentDetails");

            migrationBuilder.DropTable(
                name: "StockItems");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "StockTakeAdjustments");

            migrationBuilder.DropTable(
                name: "StockTransferDetails");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TaskComments");

            migrationBuilder.DropTable(
                name: "TrustedDevices");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserWarehouses");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "WarehouseActivities");

            migrationBuilder.DropTable(
                name: "WarehouseCellProducts");

            migrationBuilder.DropTable(
                name: "WarehouseClusters");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "ImportOrders");

            migrationBuilder.DropTable(
                name: "InventoryChecks");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "MaintenanceRequests");

            migrationBuilder.DropTable(
                name: "Productions");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "SalesOrders");

            migrationBuilder.DropTable(
                name: "StockAdjustments");

            migrationBuilder.DropTable(
                name: "StockTakeDetails");

            migrationBuilder.DropTable(
                name: "StockTransfers");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "WarehouseCells");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "StockTakes");

            migrationBuilder.DropTable(
                name: "ProductBatches");

            migrationBuilder.DropTable(
                name: "WarehouseZones");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "WarehousePositions");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
