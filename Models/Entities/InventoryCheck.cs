using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class InventoryCheck
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string CheckNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending"; // pending, in_progress, completed, cancelled

        public DateTime CheckDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        // Foreign Keys
        public int WarehouseId { get; set; }

        // Navigation Properties
        [ForeignKey("WarehouseId")]
        public virtual Warehouse? Warehouse { get; set; }

        // Inventory Check Items
        public virtual ICollection<InventoryCheckItem> InventoryCheckItems { get; set; } = new List<InventoryCheckItem>();
    }
}
