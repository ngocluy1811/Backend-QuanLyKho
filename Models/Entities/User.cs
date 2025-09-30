using System.ComponentModel.DataAnnotations;
using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Models.Entities;

public class User : BaseEntity
{
    public int CompanyId { get; set; }
    public int? DepartmentId { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [StringLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(255)]
    public string? FullName { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    public new bool IsActive { get; set; } = true;
    // public UserStatus Status { get; set; } = UserStatus.Pending; // Temporarily disabled
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockedUntil { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public DateTime? PasswordExpiresAt { get; set; }
    public bool MustChangePassword { get; set; } = false;
    public bool TwoFactorEnabled { get; set; } = false;

    [StringLength(200)]
    public string? TwoFactorSecret { get; set; }

    // Additional properties for compatibility
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public int? Level { get; set; }
    public Models.Enums.UserRole Role { get; set; } // Temporary for compatibility

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual Department? Department { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<UserWarehouse> UserWarehouses { get; set; } = new List<UserWarehouse>();
    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<SecurityEvent> SecurityEvents { get; set; } = new List<SecurityEvent>();
}
