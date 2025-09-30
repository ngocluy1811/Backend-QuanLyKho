using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class WarehouseCellProduct
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int WarehouseCellId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [MaxLength(255)]
        public string ProductName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? BatchNumber { get; set; }
        
        public int Quantity { get; set; }
        
        public int RemainingQuantity { get; set; }
        
        public decimal UnitPrice { get; set; }
        
        public decimal TotalPrice { get; set; }
        
        public DateTime? ProductionDate { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        
        [MaxLength(255)]
        public string? Supplier { get; set; }
        
        [MaxLength(50)]
        public string Unit { get; set; } = "kg";
        
        public DateTime? ArrivalDate { get; set; }
        
        [MaxLength(100)]
        public string? ArrivalBatchNumber { get; set; }
        
        [MaxLength(100)]
        public string? ContainerVehicleNumber { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
        
        // Navigation properties
        [ForeignKey("WarehouseCellId")]
        public virtual WarehouseCell WarehouseCell { get; set; } = null!;
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
