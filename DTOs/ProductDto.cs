using System.ComponentModel.DataAnnotations;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        
        [StringLength(50)]
        public string? SKU { get; set; }
        
        [StringLength(20)]
        public string? Unit { get; set; }
        
        public decimal UnitPrice { get; set; }
        public decimal MinStockLevel { get; set; }
        public decimal MaxStockLevel { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal ReservedStock { get; set; }
        public decimal AvailableStock => CurrentStock - ReservedStock;
        
        public QualityStatus QualityStatus { get; set; }
        
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastRestockedAt { get; set; }
        
        // Barcode information
        public string? BarcodeData { get; set; }
        
        // Physical properties
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        
        // Safety information
        public bool RequiresSpecialHandling { get; set; }
        public string? SafetyInstructions { get; set; }
        
        public List<ProductBatchDto> Batches { get; set; } = new();
        public List<ProductCompositionDto> Compositions { get; set; } = new();
        public List<StockItemDto> StockItems { get; set; } = new();
    }

    public class CreateProductDto
    {
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int CategoryId { get; set; }
        
        [StringLength(50)]
        public string? SKU { get; set; }
        
        [StringLength(20)]
        public string? Unit { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal MinStockLevel { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal MaxStockLevel { get; set; }
        
        public QualityStatus QualityStatus { get; set; }
        public int? SupplierId { get; set; }
        
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        
        public bool RequiresSpecialHandling { get; set; }
        public string? SafetyInstructions { get; set; }
    }

    public class UpdateProductDto
    {
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int CategoryId { get; set; }
        
        [StringLength(50)]
        public string? SKU { get; set; }
        
        [StringLength(20)]
        public string? Unit { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal MinStockLevel { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal MaxStockLevel { get; set; }
        
        public QualityStatus QualityStatus { get; set; }
        public int? SupplierId { get; set; }
        
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        
        public bool RequiresSpecialHandling { get; set; }
        public string? SafetyInstructions { get; set; }
        
        public bool IsActive { get; set; }
    }

    public class ProductStockDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal CurrentStock { get; set; }
        public decimal ReservedStock { get; set; }
        public decimal AvailableStock { get; set; }
        public decimal MinStockLevel { get; set; }
        public decimal MaxStockLevel { get; set; }
        public bool IsLowStock { get; set; }
        public bool IsOutOfStock { get; set; }
        public DateTime? LastRestockedAt { get; set; }
        public List<WarehouseStockDto> WarehouseStocks { get; set; } = new();
    }

    public class WarehouseStockDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
    }
}
