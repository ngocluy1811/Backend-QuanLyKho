namespace FertilizerWarehouseAPI.Models.Entities;

public class UserSession : BaseEntity
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public DateTime? LogoutTime { get; set; }
    public DateTime ExpiresAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}
