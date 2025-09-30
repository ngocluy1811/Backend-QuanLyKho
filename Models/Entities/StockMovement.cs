using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Models.Entities;

public class StockMovement : BaseEntity
{
    public int CompanyId { get; set; }
    public int ProductId { get; set; }
    public int BatchId { get; set; }
    public int FromPositionId { get; set; }
    public int? ToPositionId { get; set; }
    public MovementType MovementType { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int UserId { get; set; }
    public DateTime MovementDate { get; set; }
    public int WarehouseId { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ProductBatch Batch { get; set; } = null!;
    public virtual WarehousePosition FromPosition { get; set; } = null!;
    public virtual WarehousePosition? ToPosition { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual Warehouse Warehouse { get; set; } = null!;
}
