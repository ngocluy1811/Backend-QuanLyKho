using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    // Security Event DTOs
    public class SecurityEventDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(100)]
        public string EventType { get; set; } = string.Empty; // Login, Logout, PasswordChange, FailedLogin, etc.
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? IpAddress { get; set; }
        [StringLength(500)]
        public string? UserAgent { get; set; }
        [StringLength(50)]
        public string Severity { get; set; } = "Info"; // Low, Medium, High, Critical
        public DateTime EventDateTime { get; set; }
        public string? AdditionalData { get; set; } // JSON data
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDto? User { get; set; }
    }

    public class CreateSecurityEventDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(100)]
        public string EventType { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? IpAddress { get; set; }
        [StringLength(500)]
        public string? UserAgent { get; set; }
        [StringLength(50)]
        public string Severity { get; set; } = "Info";
        public string? AdditionalData { get; set; }
    }

    // Role Management DTOs
    public class RoleDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string RoleName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<PermissionDto>? Permissions { get; set; }
        public ICollection<UserDto>? Users { get; set; }
    }

    public class CreateRoleDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string RoleName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        public List<int>? PermissionIds { get; set; }
    }

    public class UpdateRoleDto
    {
        [StringLength(100)]
        public string? RoleName { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public List<int>? PermissionIds { get; set; }
    }

    // Permission DTOs
    public class PermissionDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string PermissionName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string Category { get; set; } = string.Empty; // User, Inventory, Sales, etc.
        [StringLength(100)]
        public string Resource { get; set; } = string.Empty; // Products, Orders, etc.
        [StringLength(50)]
        public string Action { get; set; } = string.Empty; // Create, Read, Update, Delete
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePermissionDto
    {
        [Required]
        [StringLength(100)]
        public string PermissionName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        [StringLength(100)]
        public string Resource { get; set; } = string.Empty;
        [StringLength(50)]
        public string Action { get; set; } = string.Empty;
    }

    public class UpdatePermissionDto
    {
        [StringLength(100)]
        public string? PermissionName { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? Category { get; set; }
        [StringLength(100)]
        public string? Resource { get; set; }
        [StringLength(50)]
        public string? Action { get; set; }
        public bool? IsActive { get; set; }
    }

    // User Session DTOs
    public class UserSessionDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(255)]
        public string SessionToken { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public DateTime ExpiryTime { get; set; }
        [StringLength(100)]
        public string? IpAddress { get; set; }
        [StringLength(500)]
        public string? UserAgent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserDto? User { get; set; }
    }

    // Audit Log DTOs
    public class AuditLogDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty; // Create, Update, Delete, View
        [Required]
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty; // Product, Order, User, etc.
        public int? EntityId { get; set; }
        public string? OldValues { get; set; } // JSON of old values
        public string? NewValues { get; set; } // JSON of new values
        [StringLength(100)]
        public string? IpAddress { get; set; }
        [StringLength(500)]
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDto? User { get; set; }
    }

    public class CreateAuditLogDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        [StringLength(100)]
        public string? IpAddress { get; set; }
        [StringLength(500)]
        public string? UserAgent { get; set; }
    }

    // System Settings DTOs
    public class SystemSettingDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string SettingKey { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? SettingValue { get; set; }
        [StringLength(100)]
        public string Category { get; set; } = string.Empty; // Security, Notification, etc.
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(50)]
        public string DataType { get; set; } = "string"; // string, int, bool, decimal, json
        public bool IsUserConfigurable { get; set; } = true;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSystemSettingDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string SettingKey { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? SettingValue { get; set; }
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(50)]
        public string DataType { get; set; } = "string";
        public bool IsUserConfigurable { get; set; } = true;
    }

    public class UpdateSystemSettingDto
    {
        [StringLength(1000)]
        public string? SettingValue { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public bool? IsUserConfigurable { get; set; }
        public bool? IsActive { get; set; }
    }
}
