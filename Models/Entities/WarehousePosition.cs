namespace FertilizerWarehouseAPI.Models.Entities;

public class WarehousePosition : BaseEntity
{
    public int WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }
    public decimal MaxCapacity { get; set; }
    public decimal CurrentCapacity { get; set; }
    public string Status { get; set; } = "Available";
    public DateTime LastUpdated { get; set; }
    public string? AssignedStaff { get; set; }

    // Navigation properties
    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public virtual ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
}
