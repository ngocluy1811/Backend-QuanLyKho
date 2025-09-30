using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class AlertDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(255)]
        public string AlertType { get; set; } = string.Empty; // LowStock, Expiry, Overdue, etc.
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        [StringLength(50)]
        public string Severity { get; set; } = "Medium"; // Low, Medium, High, Critical
        [StringLength(100)]
        public string? EntityType { get; set; } // Product, Order, Task, etc.
        public int? EntityId { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public int? ReadBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int? ResolvedBy { get; set; }
        [StringLength(1000)]
        public string? Resolution { get; set; }
        public UserDto? ReadByUser { get; set; }
        public UserDto? ResolvedByUser { get; set; }
    }

    public class CreateAlertDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(255)]
        public string AlertType { get; set; } = string.Empty;
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        [StringLength(50)]
        public string Severity { get; set; } = "Medium";
        [StringLength(100)]
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
    }

    public class UpdateAlertDto
    {
        [StringLength(1000)]
        public string? Message { get; set; }
        [StringLength(50)]
        public string? Severity { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsActive { get; set; }
        [StringLength(1000)]
        public string? Resolution { get; set; }
    }

    public class AlertRuleDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(255)]
        public string RuleName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string AlertType { get; set; } = string.Empty;
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty; // Product, Order, etc.
        [StringLength(50)]
        public string ConditionType { get; set; } = string.Empty; // LessThan, GreaterThan, Equal, etc.
        public string? ConditionValue { get; set; } // JSON configuration
        [StringLength(50)]
        public string Severity { get; set; } = "Medium";
        public bool IsEnabled { get; set; } = true;
        [StringLength(1000)]
        public string? MessageTemplate { get; set; }
        public DateTime? LastTriggered { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public UserDto? Creator { get; set; }
    }

    public class CreateAlertRuleDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(255)]
        public string RuleName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string AlertType { get; set; } = string.Empty;
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty;
        [StringLength(50)]
        public string ConditionType { get; set; } = string.Empty;
        public string? ConditionValue { get; set; }
        [StringLength(50)]
        public string Severity { get; set; } = "Medium";
        [StringLength(1000)]
        public string? MessageTemplate { get; set; }
    }

    public class UpdateAlertRuleDto
    {
        [StringLength(255)]
        public string? RuleName { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? AlertType { get; set; }
        [StringLength(100)]
        public string? EntityType { get; set; }
        [StringLength(50)]
        public string? ConditionType { get; set; }
        public string? ConditionValue { get; set; }
        [StringLength(50)]
        public string? Severity { get; set; }
        public bool? IsEnabled { get; set; }
        [StringLength(1000)]
        public string? MessageTemplate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AlertSummaryDto
    {
        public int TotalAlerts { get; set; }
        public int UnreadAlerts { get; set; }
        public int CriticalAlerts { get; set; }
        public int HighAlerts { get; set; }
        public int MediumAlerts { get; set; }
        public int LowAlerts { get; set; }
        public List<AlertDto> RecentAlerts { get; set; } = new();
    }
}
