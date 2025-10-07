using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class InventoryCheckItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InventoryCheckId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int WarehouseCellId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Location { get; set; } = string.Empty;

        public int SystemQuantity { get; set; }

        public int ActualQuantity { get; set; }

        public int Difference { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending"; // pending, matched, mismatch

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation Properties
        [ForeignKey("InventoryCheckId")]
        public virtual InventoryCheck? InventoryCheck { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("WarehouseCellId")]
        public virtual WarehouseCell? WarehouseCell { get; set; }
    }
}
