using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models
{
    public class AlertResolution
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string AlertId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string AlertType { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string ResolvedBy { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Resolution { get; set; } = string.Empty;
        
        public DateTime ResolvedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
