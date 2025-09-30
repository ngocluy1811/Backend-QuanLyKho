using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Models.Entities;

public class SalesOrder : BaseEntity
{
    public int CompanyId { get; set; }
    public int CustomerId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? RequestedDeliveryDate { get; set; }
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
    public virtual Customer Customer { get; set; } = null!;
    public virtual User? SalesPerson { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
    public virtual ICollection<SalesOrderDetail> OrderDetails { get; set; } = new List<SalesOrderDetail>();
}
