namespace FertilizerWarehouseAPI.Models.Entities;

public class StockAdjustmentDetail : BaseEntity
{
    public int AdjustmentId { get; set; }
    public int ProductId { get; set; }
    public int BatchId { get; set; }
    public int? PositionId { get; set; }
    public decimal SystemQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal DifferenceQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? Reason { get; set; }

    // Navigation properties
    public virtual StockAdjustment Adjustment { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ProductBatch Batch { get; set; } = null!;
    public virtual WarehousePosition? Position { get; set; }
}
