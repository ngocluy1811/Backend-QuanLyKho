using System.ComponentModel.DataAnnotations;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.DTOs
{
    public class StockTransferDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int FromWarehouseId { get; set; }
        [Required]
        public int ToWarehouseId { get; set; }
        [Required]
        [StringLength(100)]
        public string TransferNumber { get; set; } = string.Empty;
        [Required]
        public DateTime TransferDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public OrderStatus Status { get; set; }
        [Required]
        public int RequestedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        [StringLength(500)]
        public string? Reason { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public WarehouseDto? FromWarehouse { get; set; }
        public WarehouseDto? ToWarehouse { get; set; }
        public UserDto? RequestedByUser { get; set; }
        public UserDto? ApprovedByUser { get; set; }
        public ICollection<StockTransferDetailDto>? TransferDetails { get; set; }
    }

    public class CreateStockTransferDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int FromWarehouseId { get; set; }
        [Required]
        public int ToWarehouseId { get; set; }
        [Required]
        [StringLength(100)]
        public string TransferNumber { get; set; } = string.Empty;
        [Required]
        public DateTime TransferDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        [StringLength(500)]
        public string? Reason { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public ICollection<CreateStockTransferDetailDto>? TransferDetails { get; set; }
    }

    public class UpdateStockTransferDto
    {
        public int? FromWarehouseId { get; set; }
        public int? ToWarehouseId { get; set; }
        [StringLength(100)]
        public string? TransferNumber { get; set; }
        public DateTime? TransferDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public OrderStatus? Status { get; set; }
        [StringLength(500)]
        public string? Reason { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }

    public class StockTransferDetailDto
    {
        public int Id { get; set; }
        [Required]
        public int StockTransferId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TransferredQuantity { get; set; }
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

    public class CreateStockTransferDetailDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateStockTransferDetailDto
    {
        public int? ProductId { get; set; }
        public int? BatchId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal? Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? TransferredQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? UnitPrice { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }

    public class StockTransferSummaryDto
    {
        public int Id { get; set; }
        public string TransferNumber { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
        public string FromWarehouse { get; set; } = string.Empty;
        public string ToWarehouse { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalValue { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
    }
}
