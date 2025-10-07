using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class LoginHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Action { get; set; } // "Login", "Logout", "FailedLogin", "SuspiciousActivity"

        [Required]
        public DateTime Timestamp { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [StringLength(100)]
        public string? DeviceType { get; set; } // "Desktop", "Mobile", "Tablet"

        [StringLength(100)]
        public string? OperatingSystem { get; set; } // "Windows", "iOS", "Android", "macOS"

        [StringLength(100)]
        public string? Browser { get; set; } // "Chrome", "Firefox", "Safari", "Edge"

        [StringLength(200)]
        public string? DeviceName { get; set; } // "iPhone 13", "Dell Laptop", "Samsung Galaxy"

        [StringLength(100)]
        public string? Location { get; set; } // "Ho Chi Minh City, Vietnam"

        [StringLength(10)]
        public string? CountryCode { get; set; } // "VN", "US", "JP"

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Region { get; set; }

        [StringLength(20)]
        public string? Timezone { get; set; }

        public bool IsSuccessful { get; set; } = true;

        [StringLength(500)]
        public string? FailureReason { get; set; } // "Invalid Password", "Account Locked", "Suspicious IP"

        [StringLength(1000)]
        public string? AdditionalInfo { get; set; } // JSON string for extra data

        public bool IsSuspicious { get; set; } = false;

        [StringLength(500)]
        public string? SuspiciousReason { get; set; } // "New Device", "Unusual Location", "Multiple Failed Attempts"

        [StringLength(50)]
        public string? SessionId { get; set; }

        public DateTime? SessionExpiry { get; set; }

        [StringLength(100)]
        public string? LoginMethod { get; set; } // "Password", "2FA", "SSO", "Biometric"

        public bool IsActiveSession { get; set; } = false;

        // Security flags
        public bool IsFromNewDevice { get; set; } = false;
        public bool IsFromNewLocation { get; set; } = false;
        public bool IsFromNewIp { get; set; } = false;
        public bool IsOffHoursLogin { get; set; } = false; // Login outside business hours
        public bool IsRapidLogin { get; set; } = false; // Multiple logins in short time

        // Risk score (0-100, higher = more risky)
        public int RiskScore { get; set; } = 0;

        [StringLength(50)]
        public string? RiskLevel { get; set; } // "Low", "Medium", "High", "Critical"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
