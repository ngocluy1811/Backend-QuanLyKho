using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class SecurityEvent : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string EventType { get; set; } = string.Empty; // 'Login', 'Logout', 'FailedLogin', 'PasswordChange'

        public int? UserId { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [StringLength(1000)]
        public string? Details { get; set; }

        [StringLength(20)]
        public string? Severity { get; set; } // 'Low', 'Medium', 'High', 'Critical'

        public DateTime EventTime { get; set; } = DateTime.UtcNow;
        public bool IsBlocked { get; set; } = false;

        // Navigation properties
        public virtual User? User { get; set; }
    }
}
