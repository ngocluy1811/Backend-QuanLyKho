namespace FertilizerWarehouseAPI.Models.Entities;

public class UserPermission : BaseEntity
{
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public bool IsGranted { get; set; } = true;
    public DateTime GrantedAt { get; set; }
    public int? GrantedBy { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}
