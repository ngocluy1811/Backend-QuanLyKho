using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Type { get; set; } = "info"; // info, warning, error, success
        
        [StringLength(20)]
        public string Priority { get; set; } = "medium"; // low, medium, high, critical
        
        public bool IsRead { get; set; } = false;
        
        public int? UserId { get; set; } // null = global notification
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Models.Entities.User? User { get; set; }
    }
}
