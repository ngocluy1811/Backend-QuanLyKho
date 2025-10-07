using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class TrustedDevice
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; }

        [Required]
        [StringLength(100)]
        public string DeviceType { get; set; } // "Desktop", "Mobile", "Tablet"

        [Required]
        [StringLength(100)]
        public string OperatingSystem { get; set; }

        [Required]
        [StringLength(100)]
        public string Browser { get; set; }

        [Required]
        [StringLength(500)]
        public string UserAgent { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(10)]
        public string? CountryCode { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Region { get; set; }

        [Required]
        public DateTime FirstSeen { get; set; } = DateTime.UtcNow;

        public DateTime LastSeen { get; set; } = DateTime.UtcNow;

        public int LoginCount { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public bool IsBlocked { get; set; } = false;

        [StringLength(500)]
        public string? BlockReason { get; set; }

        public DateTime? BlockedAt { get; set; }

        [StringLength(100)]
        public string? DeviceFingerprint { get; set; } // Unique device identifier

        [StringLength(50)]
        public string? TrustLevel { get; set; } = "Medium"; // "Low", "Medium", "High", "Trusted"

        public int RiskScore { get; set; } = 0;

        [StringLength(1000)]
        public string? AdditionalInfo { get; set; } // JSON string for extra data

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
