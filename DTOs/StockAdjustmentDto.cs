using System.ComponentModel.DataAnnotations;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.DTOs
{
    public class StockAdjustmentDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        [StringLength(100)]
        public string AdjustmentNumber { get; set; } = string.Empty;
        [Required]
        public DateTime AdjustmentDate { get; set; }
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public int AdjustedBy { get; set; }
        public OrderStatus Status { get; set; } // Pending, Approved, Rejected
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public UserDto? AdjustedByUser { get; set; }
        public ICollection<StockAdjustmentDetailDto>? AdjustmentDetails { get; set; }
    }

    public class CreateStockAdjustmentDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        [StringLength(100)]
        public string AdjustmentNumber { get; set; } = string.Empty;
        [Required]
        public DateTime AdjustmentDate { get; set; }
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Notes { get; set; }
        public ICollection<CreateStockAdjustmentDetailDto>? AdjustmentDetails { get; set; }
    }

    public class UpdateStockAdjustmentDto
    {
        public int? WarehouseId { get; set; }
        [StringLength(100)]
        public string? AdjustmentNumber { get; set; }
        public DateTime? AdjustmentDate { get; set; }
        [StringLength(500)]
        public string? Reason { get; set; }
        public OrderStatus? Status { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }

    public class StockAdjustmentDetailDto
    {
        public int Id { get; set; }
        [Required]
        public int StockAdjustmentId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal OldQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal NewQuantity { get; set; }
        public decimal AdjustmentQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ProductDto? Product { get; set; }
        public ProductBatchDto? Batch { get; set; }
    }

    public class CreateStockAdjustmentDetailDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal OldQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal NewQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateStockAdjustmentDetailDto
    {
        public int? ProductId { get; set; }
        public int? BatchId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? OldQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? NewQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? UnitPrice { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }

    public class StockAdjustmentSummaryDto
    {
        public int Id { get; set; }
        public string AdjustmentNumber { get; set; } = string.Empty;
        public DateTime AdjustmentDate { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalValue { get; set; }
        public string AdjustedBy { get; set; } = string.Empty;
    }
}
