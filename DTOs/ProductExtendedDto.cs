using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs;

// Product Extended DTOs
public class ProductInventoryDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal MinStockLevel { get; set; }
    public decimal MaxStockLevel { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ProductStockLocationDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public int CellId { get; set; }
    public string CellCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class ProductPriceHistoryDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public decimal PriceChange { get; set; }
    public decimal PriceChangePercent { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public class ProductPriceChangeDto
{
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public decimal ChangeAmount { get; set; }
    public decimal ChangePercent { get; set; }
    public DateTime ChangedAt { get; set; }
}

public class ProductSupplierDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal SupplierPrice { get; set; }
    public string Currency { get; set; } = "VND";
    public int LeadTime { get; set; } // days
    public decimal MinOrderQuantity { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ProductStatsDto
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public decimal AveragePrice { get; set; }
    public int ProductsByCategory { get; set; }
    public int ProductsBySupplier { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class ProductMovementHistoryDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string MovementType { get; set; } = string.Empty; // In, Out, Transfer
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    public string? Reference { get; set; } // Order number, etc.
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ProductTrendDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly
    public DateTime Date { get; set; }
    public decimal QuantitySold { get; set; }
    public decimal Revenue { get; set; }
    public decimal Profit { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public class ProductQualityCheckDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public string CheckType { get; set; } = string.Empty; // Incoming, Outgoing, Random
    public string Status { get; set; } = string.Empty; // Pass, Fail, Pending
    public decimal? PhLevel { get; set; }
    public decimal? MoistureContent { get; set; }
    public decimal? NitrogenContent { get; set; }
    public decimal? PhosphorusContent { get; set; }
    public decimal? PotassiumContent { get; set; }
    public string? Notes { get; set; }
    public string CheckedBy { get; set; } = string.Empty;
    public DateTime CheckedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class CreateProductQualityCheckDto
{
    [Required(ErrorMessage = "ID sản phẩm là bắt buộc.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Số lô là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Số lô không được vượt quá 50 ký tự.")]
    public string BatchNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loại kiểm tra là bắt buộc.")]
    public string CheckType { get; set; } = string.Empty;

    [Range(0, 14, ErrorMessage = "Độ pH phải từ 0-14.")]
    public decimal? PhLevel { get; set; }

    [Range(0, 100, ErrorMessage = "Độ ẩm phải từ 0-100%.")]
    public decimal? MoistureContent { get; set; }

    [Range(0, 100, ErrorMessage = "Hàm lượng N phải từ 0-100%.")]
    public decimal? NitrogenContent { get; set; }

    [Range(0, 100, ErrorMessage = "Hàm lượng P phải từ 0-100%.")]
    public decimal? PhosphorusContent { get; set; }

    [Range(0, 100, ErrorMessage = "Hàm lượng K phải từ 0-100%.")]
    public decimal? PotassiumContent { get; set; }

    public string? Notes { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class QualityTestResultDto
{
    public int Id { get; set; }
    public int QualityCheckId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string TestMethod { get; set; } = string.Empty;
    public decimal? ExpectedValue { get; set; }
    public decimal? ActualValue { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty; // Pass, Fail, Warning
    public string? Notes { get; set; }
    public string TestedBy { get; set; } = string.Empty;
    public DateTime TestedAt { get; set; }
}

public class CreateQualityTestResultDto
{
    [Required(ErrorMessage = "ID kiểm tra chất lượng là bắt buộc.")]
    public int QualityCheckId { get; set; }

    [Required(ErrorMessage = "Tên test là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên test không được vượt quá 100 ký tự.")]
    public string TestName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Phương pháp test không được vượt quá 100 ký tự.")]
    public string? TestMethod { get; set; }

    public decimal? ExpectedValue { get; set; }
    public decimal? ActualValue { get; set; }

    [StringLength(20, ErrorMessage = "Đơn vị không được vượt quá 20 ký tự.")]
    public string? Unit { get; set; }

    [Required(ErrorMessage = "Kết quả là bắt buộc.")]
    public string Result { get; set; } = string.Empty;

    public string? Notes { get; set; }
}

public class ProductLocationDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public int CellId { get; set; }
    public string CellCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Status { get; set; } = string.Empty;
}
