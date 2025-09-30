namespace FertilizerWarehouseAPI.Models.Entities;

public class StockAdjustment : BaseEntity
{
    public int CompanyId { get; set; }
    public string AdjustmentNumber { get; set; } = string.Empty;
    public DateTime AdjustmentDate { get; set; }
    public string AdjustmentType { get; set; } = string.Empty; // Increase, Decrease
    public string Reason { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public int? ApprovedBy { get; set; }
    public string Status { get; set; } = "Pending";
    public string? EvidencePhoto { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual User? Approver { get; set; }
    public virtual User Creator { get; set; } = null!;
    public virtual ICollection<StockAdjustmentDetail> StockAdjustmentDetails { get; set; } = new List<StockAdjustmentDetail>();
}
