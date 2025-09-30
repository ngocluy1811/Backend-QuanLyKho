using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Models.Entities;

public class PurchaseOrder : BaseEntity
{
    public int CompanyId { get; set; }
    public int SupplierId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public OrderStatus Status { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? WarehouseId { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual Supplier Supplier { get; set; } = null!;
    public virtual User? Approver { get; set; }
    public virtual User Creator { get; set; } = null!;
    public virtual Warehouse? Warehouse { get; set; }
    public virtual ICollection<PurchaseOrderDetail> OrderDetails { get; set; } = new List<PurchaseOrderDetail>();
}
