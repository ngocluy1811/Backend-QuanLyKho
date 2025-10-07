using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class MaintenanceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string MaintenanceType { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string AssignedTo { get; set; } = string.Empty;

        public DateTime ScheduledDate { get; set; }

        [MaxLength(100)]
        public string? EstimatedDuration { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending"; // pending, in_progress, completed, cancelled

        public int? Progress { get; set; } // 0-100

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public DateTime? CompletedAt { get; set; }

        // Foreign Keys
        public int WarehouseId { get; set; }
        public int? WarehouseCellId { get; set; }

        // Navigation Properties
        [ForeignKey("WarehouseId")]
        public virtual Warehouse? Warehouse { get; set; }

        [ForeignKey("WarehouseCellId")]
        public virtual WarehouseCell? WarehouseCell { get; set; }

        // Maintenance History
        public virtual ICollection<MaintenanceHistory> MaintenanceHistories { get; set; } = new List<MaintenanceHistory>();
    }
}
