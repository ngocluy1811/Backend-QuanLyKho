using System.ComponentModel.DataAnnotations;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.DTOs
{
    public class StockTakeDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        [StringLength(100)]
        public string StockTakeNumber { get; set; } = string.Empty;
        [Required]
        public DateTime StockTakeDate { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        public OrderStatus Status { get; set; } // Planned, InProgress, Completed, Cancelled
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public UserDto? CreatedByUser { get; set; }
        public ICollection<StockTakeDetailDto>? StockTakeDetails { get; set; }
        // Additional properties for compatibility
        public string WarehouseName { get; set; } = string.Empty; // For compatibility
        public string CreatedByName { get; set; } = string.Empty; // For compatibility
        public int TotalItems { get; set; } = 0; // For compatibility
        public int CheckedItems { get; set; } = 0; // For compatibility
        public int Discrepancies { get; set; } = 0; // For compatibility
        public List<string> Locations { get; set; } = new List<string>(); // For compatibility
        public List<string> Categories { get; set; } = new List<string>(); // For compatibility
        public List<string> AssignedTo { get; set; } = new List<string>(); // For compatibility
        public string Details { get; set; } = string.Empty; // For compatibility
    }

    public class CreateStockTakeDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        [StringLength(100)]
        public string StockTakeNumber { get; set; } = string.Empty;
        [Required]
        public DateTime StockTakeDate { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        // Additional properties for compatibility
        public List<string>? Locations { get; set; } // For compatibility
        public List<string>? Categories { get; set; } // For compatibility
        public List<string>? AssignedTo { get; set; } // For compatibility
    }

    public class UpdateStockTakeDto
    {
        public int? WarehouseId { get; set; }
        [StringLength(100)]
        public string? StockTakeNumber { get; set; }
        public DateTime? StockTakeDate { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public OrderStatus? Status { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
        // Additional properties for compatibility
        public List<string>? Locations { get; set; } // For compatibility
        public List<string>? Categories { get; set; } // For compatibility
        public List<string>? AssignedTo { get; set; } // For compatibility
    }

    public class StockTakeDetailDto
    {
        public int Id { get; set; }
        [Required]
        public int StockTakeId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Required]
        public int PositionId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal SystemQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal ActualQuantity { get; set; }
        public decimal Variance { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ProductDto? Product { get; set; }
        public ProductBatchDto? Batch { get; set; }
        public WarehousePositionDto? Position { get; set; }
        // Additional properties for compatibility
        public string ProductName { get; set; } = string.Empty; // For compatibility
        public string ProductCode { get; set; } = string.Empty; // For compatibility
        public string BatchNumber { get; set; } = string.Empty; // For compatibility
        public string PositionCode { get; set; } = string.Empty; // For compatibility
        public DateTime? CheckedAt { get; set; } // For compatibility
        public string CheckedBy { get; set; } = string.Empty; // For compatibility
    }

    public class CreateStockTakeDetailDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Required]
        public int PositionId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal SystemQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal ActualQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateStockTakeDetailDto
    {
        public int? ProductId { get; set; }
        public int? BatchId { get; set; }
        public int? PositionId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? SystemQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? ActualQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? UnitPrice { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }

    public class StockTakeSummaryDto
    {
        public int Id { get; set; }
        public string StockTakeNumber { get; set; } = string.Empty;
        public DateTime StockTakeDate { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public int TotalItems { get; set; }
        public int ItemsWithVariance { get; set; }
        public decimal TotalVarianceValue { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
    }

    public class StockTakeVarianceDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string PositionCode { get; set; } = string.Empty;
        public decimal SystemQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VarianceValue { get; set; }
        public string? Notes { get; set; }
    }
}