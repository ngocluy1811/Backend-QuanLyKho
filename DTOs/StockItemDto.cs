using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class StockItemDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Required]
        public int PositionId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal CurrentQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal ReservedQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal AvailableQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalValue { get; set; }
        public DateTime? LastMovementDate { get; set; }
        public DateTime? LastCountDate { get; set; }
        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Quarantined, Expired, Reserved
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public ProductDto? Product { get; set; }
        public ProductBatchDto? Batch { get; set; }
        public WarehousePositionDto? Position { get; set; }
    }

    public class CreateStockItemDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Required]
        public int PositionId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal CurrentQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }
    }

    public class UpdateStockItemDto
    {
        public int? WarehouseId { get; set; }
        public int? PositionId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? CurrentQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? ReservedQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? UnitCost { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }

    public class StockLevelDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal TotalStock { get; set; }
        public decimal AvailableStock { get; set; }
        public decimal ReservedStock { get; set; }
        public decimal MinStockLevel { get; set; }
        public decimal MaxStockLevel { get; set; }
        public string StockStatus { get; set; } = string.Empty; // OK, Low, Out, Over
        public int WarehouseCount { get; set; }
        public List<WarehouseStockDto> WarehouseStock { get; set; } = new();
    }


    public class BatchStockDto
    {
        public int BatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public int DaysToExpiry { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PositionCode { get; set; } = string.Empty;
    }

    public class StockSummaryDto
    {
        public int TotalProducts { get; set; }
        public int ProductsInStock { get; set; }
        public int ProductsOutOfStock { get; set; }
        public int ProductsLowStock { get; set; }
        public int ProductsOverStock { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal TotalAvailableValue { get; set; }
        public decimal TotalReservedValue { get; set; }
        public int TotalBatches { get; set; }
        public int BatchesExpiringSoon { get; set; }
        public int ExpiredBatches { get; set; }
    }

    public class StockMovementAnalysisDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal CurrentStock { get; set; }
        public decimal AverageMonthlyConsumption { get; set; }
        public decimal MonthsOfStockRemaining { get; set; }
        public decimal ReorderPoint { get; set; }
        public decimal OptimalOrderQuantity { get; set; }
        public string RecommendedAction { get; set; } = string.Empty; // Order, Monitor, Reduce
        public DateTime? LastOrderDate { get; set; }
        public DateTime? SuggestedOrderDate { get; set; }
    }

    public class StockLocationDto
    {
        public int StockItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string ZoneName { get; set; } = string.Empty;
        public string PositionCode { get; set; } = string.Empty;
        public string FullLocation { get; set; } = string.Empty; // Warehouse > Zone > Position
        public DateTime? LastMovementDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class StockAgeAnalysisDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime ProductionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int AgeInDays { get; set; }
        public int DaysToExpiry { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
        public string AgeCategory { get; set; } = string.Empty; // Fresh, Aging, NearExpiry, Expired
        public string Recommendation { get; set; } = string.Empty;
    }
}