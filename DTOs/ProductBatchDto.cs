using System.ComponentModel.DataAnnotations;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.DTOs
{
    public class ProductBatchDto
    {
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [StringLength(100)]
        public string BatchNumber { get; set; } = string.Empty;
        [Required]
        public DateTime ProductionDate { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        public int? SupplierId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal InitialQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal CurrentQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }
        public QualityStatus QualityStatus { get; set; }
        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Expired, Quarantined, Used
        public int? PositionId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ProductDto? Product { get; set; }
        public SupplierDto? Supplier { get; set; }
        public WarehousePositionDto? Position { get; set; }
    }

    public class CreateProductBatchDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [StringLength(100)]
        public string BatchNumber { get; set; } = string.Empty;
        [Required]
        public DateTime ProductionDate { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        public int? SupplierId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal InitialQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }
        public QualityStatus QualityStatus { get; set; } = QualityStatus.Passed;
        public int? PositionId { get; set; }
    }

    public class UpdateProductBatchDto
    {
        [StringLength(100)]
        public string? BatchNumber { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? SupplierId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal? InitialQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? CurrentQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? UnitCost { get; set; }
        public QualityStatus? QualityStatus { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }
        public int? PositionId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ProductBatchSummaryDto
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public DateTime ProductionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal CurrentQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public QualityStatus QualityStatus { get; set; }
        public string? PositionCode { get; set; }
        public string? WarehouseName { get; set; }
    }

    public class BatchStockLevelDto
    {
        public int BatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal CurrentQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysToExpiry { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}