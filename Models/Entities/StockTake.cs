namespace FertilizerWarehouseAPI.Models.Entities;

public class StockTake : BaseEntity
{
    public int CompanyId { get; set; }
    public string StockTakeNumber { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? WarehouseId { get; set; }
    public int? ZoneId { get; set; }
    public string Status { get; set; } = "InProgress";
    public int SupervisorId { get; set; }
    public int TotalItems { get; set; }
    public decimal VarianceValue { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual Warehouse? Warehouse { get; set; }
    public virtual WarehouseZone? Zone { get; set; }
    public virtual User Supervisor { get; set; } = null!;
    public virtual ICollection<StockTakeDetail> StockTakeDetails { get; set; } = new List<StockTakeDetail>();
}
