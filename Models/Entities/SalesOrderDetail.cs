namespace FertilizerWarehouseAPI.Models.Entities;

public class SalesOrderDetail : BaseEntity
{
    public int SalesOrderId { get; set; }
    public int ProductId { get; set; }
    public int? BatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ShippedQuantity { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual SalesOrder SalesOrder { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ProductBatch? Batch { get; set; }
}
