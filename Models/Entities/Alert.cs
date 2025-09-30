namespace FertilizerWarehouseAPI.Models.Entities;

public class Alert : BaseEntity
{
    public int CompanyId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low"; // Low, Medium, High, Critical
    public string Status { get; set; } = "Active";
    public string TriggerCondition { get; set; } = string.Empty;
    public string? AffectedEntity { get; set; }
    public int? AffectedEntityId { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int? ResolvedBy { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual User? Resolver { get; set; }
}
