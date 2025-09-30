namespace FertilizerWarehouseAPI.Models.Entities;

public class SystemSetting : BaseEntity
{
    public int? CompanyId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsGlobal { get; set; }

    // Navigation properties
    public virtual Company? Company { get; set; }
    public virtual User? Updater { get; set; }
}
