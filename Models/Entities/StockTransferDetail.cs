namespace FertilizerWarehouseAPI.Models.Entities;

public class StockTransferDetail : BaseEntity
{
    public int StockTransferId { get; set; }
    public int ProductId { get; set; }
    public int BatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal TransferredQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual StockTransfer StockTransfer { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ProductBatch Batch { get; set; } = null!;
}
