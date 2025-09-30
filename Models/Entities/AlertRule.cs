namespace FertilizerWarehouseAPI.Models.Entities;

public class AlertRule : BaseEntity
{
    public int CompanyId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public decimal? Threshold { get; set; }
    public string NotificationChannels { get; set; } = string.Empty; // JSON array
    public string Recipients { get; set; } = string.Empty; // JSON array

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
}
