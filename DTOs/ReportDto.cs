using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class ReportDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(255)]
        public string ReportName { get; set; } = string.Empty;
        [StringLength(100)]
        public string ReportType { get; set; } = string.Empty; // Inventory, Sales, Purchase, Production, etc.
        [StringLength(1000)]
        public string? Description { get; set; }
        [Required]
        public DateTime GeneratedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public int GeneratedBy { get; set; }
        [StringLength(50)]
        public string Status { get; set; } = "Generated"; // Generated, Archived, Deleted
        [StringLength(255)]
        public string? FilePath { get; set; }
        [StringLength(50)]
        public string? FileFormat { get; set; } // PDF, Excel, CSV
        [Range(0, long.MaxValue)]
        public long? FileSize { get; set; }
        public string? ReportData { get; set; } // JSON data for the report
        public string? Parameters { get; set; } // JSON parameters used to generate the report
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserDto? GeneratedByUser { get; set; }
    }

    public class CreateReportDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(255)]
        public string ReportName { get; set; } = string.Empty;
        [StringLength(100)]
        public string ReportType { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [StringLength(50)]
        public string? FileFormat { get; set; } = "PDF";
        public string? Parameters { get; set; }
    }

    public class ReportParameterDto
    {
        [Required]
        [StringLength(100)]
        public string ParameterName { get; set; } = string.Empty;
        [StringLength(50)]
        public string ParameterType { get; set; } = string.Empty; // string, int, date, bool
        public object? ParameterValue { get; set; }
        [StringLength(255)]
        public string? DisplayName { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public bool IsRequired { get; set; } = false;
    }

    // Inventory Reports
    public class InventoryReportDto
    {
        public DateTime ReportDate { get; set; }
        public List<InventoryItemDto> Items { get; set; } = new();
        public InventorySummaryDto Summary { get; set; } = new();
    }

    public class InventoryItemDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal CurrentStock { get; set; }
        public decimal MinStockLevel { get; set; }
        public decimal MaxStockLevel { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string Status { get; set; } = string.Empty; // OK, Low Stock, Out of Stock, Overstock
        public DateTime? LastMovementDate { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
    }

    public class InventorySummaryDto
    {
        public int TotalProducts { get; set; }
        public int ProductsInStock { get; set; }
        public int ProductsOutOfStock { get; set; }
        public int ProductsLowStock { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal AverageStockLevel { get; set; }
    }

    // Sales Reports
    public class SalesReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SalesItemDto> Items { get; set; } = new();
        public SalesSummaryDto Summary { get; set; } = new();
    }

    public class SalesItemDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string SalesPerson { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class SalesSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalCustomers { get; set; }
        public string TopSellingProduct { get; set; } = string.Empty;
        public string TopCustomer { get; set; } = string.Empty;
    }

    // Purchase Reports
    public class PurchaseReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<PurchaseItemDto> Items { get; set; } = new();
        public PurchaseSummaryDto Summary { get; set; } = new();
    }

    public class PurchaseItemDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? DeliveryDate { get; set; }
    }

    public class PurchaseSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalSuppliers { get; set; }
        public string TopSupplier { get; set; } = string.Empty;
        public string MostPurchasedProduct { get; set; } = string.Empty;
    }

    // Stock Movement Reports
    public class StockMovementReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<StockMovementItemDto> Items { get; set; } = new();
        public StockMovementSummaryDto Summary { get; set; } = new();
    }

    public class StockMovementItemDto
    {
        public int MovementId { get; set; }
        public DateTime MovementDate { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string FromLocation { get; set; } = string.Empty;
        public string ToLocation { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class StockMovementSummaryDto
    {
        public int TotalMovements { get; set; }
        public int StockInMovements { get; set; }
        public int StockOutMovements { get; set; }
        public int TransferMovements { get; set; }
        public int AdjustmentMovements { get; set; }
        public decimal TotalStockIn { get; set; }
        public decimal TotalStockOut { get; set; }
    }
}