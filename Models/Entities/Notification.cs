namespace FertilizerWarehouseAPI.Models.Entities;

public class Notification : BaseEntity
{
    public int CompanyId { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty; // e.g., 'System', 'Alert', 'Task', 'Info'
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; }
    public string? EntityType { get; set; } // e.g., 'Task', 'Order', 'Product'
    public int? EntityId { get; set; }
    public string? Link { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
