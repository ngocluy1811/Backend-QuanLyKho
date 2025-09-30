namespace FertilizerWarehouseAPI.Models.Entities;

public class StockTakeDetail : BaseEntity
{
    public int StockTakeId { get; set; }
    public int ProductId { get; set; }
    public int BatchId { get; set; }
    public int? PositionId { get; set; }
    public decimal SystemQuantity { get; set; }
    public decimal CountedQuantity { get; set; }
    public decimal VarianceQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal VarianceValue { get; set; }
    public int CountedBy { get; set; }
    public DateTime CountedAt { get; set; }
    public string? Photo { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Pending";

    // Navigation properties
    public virtual StockTake StockTake { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ProductBatch Batch { get; set; } = null!;
    public virtual WarehousePosition? Position { get; set; }
    public virtual User Counter { get; set; } = null!;
}
