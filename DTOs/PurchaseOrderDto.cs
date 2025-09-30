using System.ComponentModel.DataAnnotations;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.DTOs
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int SupplierId { get; set; }
        [Required]
        [StringLength(100)]
        public string OrderNumber { get; set; } = string.Empty;
        [Required]
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal GrandTotal { get; set; }
        public OrderStatus Status { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? WarehouseId { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public SupplierDto? Supplier { get; set; }
        public UserDto? Approver { get; set; }
        public UserDto? Creator { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public ICollection<PurchaseOrderDetailDto>? OrderDetails { get; set; }
    }

    public class CreatePurchaseOrderDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int SupplierId { get; set; }
        [Required]
        [StringLength(100)]
        public string OrderNumber { get; set; } = string.Empty;
        [Required]
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal GrandTotal { get; set; }
        public int? WarehouseId { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public ICollection<CreatePurchaseOrderDetailDto>? OrderDetails { get; set; }
    }

    public class UpdatePurchaseOrderDto
    {
        public int? SupplierId { get; set; }
        [StringLength(100)]
        public string? OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? TotalAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? TaxAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? DiscountAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? GrandTotal { get; set; }
        public OrderStatus? Status { get; set; }
        public int? WarehouseId { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }

    public class PurchaseOrderDetailDto
    {
        public int Id { get; set; }
        [Required]
        public int PurchaseOrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [Range(0, 1)]
        public decimal TaxRate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal ReceivedQuantity { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ProductDto? Product { get; set; }
    }

    public class CreatePurchaseOrderDetailDto
    {
        [Required]
        public int ProductId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [Range(0, 1)]
        public decimal TaxRate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdatePurchaseOrderDetailDto
    {
        public int? ProductId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal? Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? UnitPrice { get; set; }
        [Range(0, 1)]
        public decimal? TaxRate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? TotalAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? ReceivedQuantity { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }
}
