using System.ComponentModel.DataAnnotations;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.DTOs
{
    public class SalesOrderDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        [StringLength(100)]
        public string OrderNumber { get; set; } = string.Empty;
        [Required]
        public DateTime OrderDate { get; set; }
        public DateTime? RequestedDeliveryDate { get; set; }
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
        public CustomerDto? Customer { get; set; }
        public UserDto? SalesPerson { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public ICollection<SalesOrderDetailDto>? OrderDetails { get; set; }
    }

    public class CreateSalesOrderDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        [StringLength(100)]
        public string OrderNumber { get; set; } = string.Empty;
        [Required]
        public DateTime OrderDate { get; set; }
        public DateTime? RequestedDeliveryDate { get; set; }
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
        public ICollection<CreateSalesOrderDetailDto>? OrderDetails { get; set; }
    }

    public class UpdateSalesOrderDto
    {
        public int? CustomerId { get; set; }
        [StringLength(100)]
        public string? OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequestedDeliveryDate { get; set; }
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

    public class SalesOrderDetailDto
    {
        public int Id { get; set; }
        [Required]
        public int SalesOrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public int? BatchId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [Range(0, 1)]
        public decimal TaxRate { get; set; }
        [Range(0, 1)]
        public decimal DiscountRate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal ShippedQuantity { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ProductDto? Product { get; set; }
        public ProductBatchDto? Batch { get; set; }
    }

    public class CreateSalesOrderDetailDto
    {
        [Required]
        public int ProductId { get; set; }
        public int? BatchId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [Range(0, 1)]
        public decimal TaxRate { get; set; }
        [Range(0, 1)]
        public decimal DiscountRate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateSalesOrderDetailDto
    {
        public int? ProductId { get; set; }
        public int? BatchId { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal? Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? UnitPrice { get; set; }
        [Range(0, 1)]
        public decimal? TaxRate { get; set; }
        [Range(0, 1)]
        public decimal? DiscountRate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? TotalAmount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? ShippedQuantity { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }
}
