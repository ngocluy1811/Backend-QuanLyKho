using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        [StringLength(50)]
        public string NotificationType { get; set; } = "Info"; // System, Alert, Task, Info
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; }
        [StringLength(100)]
        public string? EntityType { get; set; } // Task, Order, Product
        public int? EntityId { get; set; }
        [StringLength(500)]
        public string? Link { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CompanyDto? Company { get; set; }
        public UserDto? User { get; set; }
    }

    public class CreateNotificationDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        [StringLength(50)]
        public string NotificationType { get; set; } = "Info";
        [StringLength(100)]
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        [StringLength(500)]
        public string? Link { get; set; }
    }

    public class UpdateNotificationDto
    {
        [StringLength(1000)]
        public string? Message { get; set; }
        [StringLength(50)]
        public string? NotificationType { get; set; }
        public bool? IsRead { get; set; }
        [StringLength(100)]
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        [StringLength(500)]
        public string? Link { get; set; }
        public bool? IsActive { get; set; }
    }

    public class MarkNotificationReadDto
    {
        [Required]
        public int NotificationId { get; set; }
        public bool IsRead { get; set; } = true;
    }

    public class BulkNotificationDto
    {
        [Required]
        public List<int> UserIds { get; set; } = new List<int>();
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        [StringLength(50)]
        public string NotificationType { get; set; } = "Info";
        [StringLength(100)]
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        [StringLength(500)]
        public string? Link { get; set; }
    }
}