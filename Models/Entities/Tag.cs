namespace FertilizerWarehouseAPI.Models.Entities;

public class Tag : BaseEntity
{
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual ICollection<EntityTag> EntityTags { get; set; } = new List<EntityTag>();
}
