using System.ComponentModel.DataAnnotations;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.DTOs
{
    public class StockMovementDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Required]
        public int FromPositionId { get; set; }
        public int? ToPositionId { get; set; }
        public MovementType MovementType { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [Required]
        public int UserId { get; set; }
        public DateTime MovementDate { get; set; }
        [Required]
        public int WarehouseId { get; set; }
        [StringLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ProductDto? Product { get; set; }
        public ProductBatchDto? Batch { get; set; }
        public WarehousePositionDto? FromPosition { get; set; }
        public WarehousePositionDto? ToPosition { get; set; }
        public UserDto? User { get; set; }
        public WarehouseDto? Warehouse { get; set; }
    }

    public class CreateStockMovementDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Required]
        public int FromPositionId { get; set; }
        public int? ToPositionId { get; set; }
        public MovementType MovementType { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [Required]
        public int WarehouseId { get; set; }
        [StringLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class UpdateStockMovementDto
    {
        public int? ToPositionId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal? Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? UnitPrice { get; set; }
        [StringLength(100)]
        public string? ReferenceNumber { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }

}
