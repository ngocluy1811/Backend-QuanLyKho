namespace FertilizerWarehouseAPI.Models.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}