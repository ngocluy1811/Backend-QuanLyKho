namespace FertilizerWarehouseAPI.Models.Entities;

public class EntityTag : BaseEntity
{
    public int TagId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public int AssignedBy { get; set; }
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    public virtual Tag Tag { get; set; } = null!;
    public virtual User Assigner { get; set; } = null!;
}
