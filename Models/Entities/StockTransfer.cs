using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Models.Entities;

public class StockTransfer : BaseEntity
{
    public int CompanyId { get; set; }
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public OrderStatus Status { get; set; }
    public int RequestedBy { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual Warehouse FromWarehouse { get; set; } = null!;
    public virtual Warehouse ToWarehouse { get; set; } = null!;
    public virtual User RequestedByUser { get; set; } = null!;
    public virtual User? ApprovedByUser { get; set; }
    public virtual ICollection<StockTransferDetail> TransferDetails { get; set; } = new List<StockTransferDetail>();
}
