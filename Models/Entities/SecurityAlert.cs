using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class SecurityAlert
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string AlertType { get; set; } // "SuspiciousLogin", "MultipleFailedAttempts", "NewDevice", "UnusualLocation", "AccountLocked"

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string Severity { get; set; } // "Low", "Medium", "High", "Critical"

        [Required]
        public DateTime AlertTime { get; set; } = DateTime.UtcNow;

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(100)]
        public string? DeviceInfo { get; set; }

        public bool IsRead { get; set; } = false;

        public bool IsResolved { get; set; } = false;

        public DateTime? ResolvedAt { get; set; }

        [StringLength(500)]
        public string? ResolutionNotes { get; set; }

        [StringLength(50)]
        public string? Status { get; set; } = "Active"; // "Active", "Investigating", "Resolved", "FalsePositive"

        [StringLength(1000)]
        public string? AdditionalData { get; set; } // JSON string for extra information

        [StringLength(50)]
        public string? ActionTaken { get; set; } // "AccountLocked", "PasswordReset", "2FAEnabled", "None"

        public bool RequiresUserAction { get; set; } = false;

        [StringLength(500)]
        public string? UserActionRequired { get; set; } // "VerifyIdentity", "ChangePassword", "Enable2FA"

        public DateTime? UserActionDeadline { get; set; }

        public bool IsNotified { get; set; } = false;

        public DateTime? NotificationSentAt { get; set; }

        [StringLength(100)]
        public string? NotificationMethod { get; set; } // "Email", "SMS", "Push", "InApp"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
