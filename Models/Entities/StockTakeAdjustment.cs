namespace FertilizerWarehouseAPI.Models.Entities;

public class StockTakeAdjustment : BaseEntity
{
    public int StockTakeDetailId { get; set; }
    public int AdjustmentId { get; set; }
    public string Status { get; set; } = "Pending";

    // Navigation properties
    public virtual StockTakeDetail StockTakeDetail { get; set; } = null!;
    public virtual StockAdjustment Adjustment { get; set; } = null!;
}
