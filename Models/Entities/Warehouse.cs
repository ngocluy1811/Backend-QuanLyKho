using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        
        [Range(1, 50)]
        public int Width { get; set; }
        
        [Range(1, 50)]
        public int Height { get; set; }
        
        public string Size => $"{Width}x{Height}";
        
        public int TotalPositions => Width * Height;
        
        [StringLength(50)]
        public string Status { get; set; } = "Active";
        
        public int? CompanyId { get; set; }
        
        public int? ManagerId { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual Company? Company { get; set; }
        public virtual User? Manager { get; set; }
        public virtual ICollection<WarehouseCell> Cells { get; set; } = new List<WarehouseCell>();
        public virtual ICollection<WarehouseCluster> Clusters { get; set; } = new List<WarehouseCluster>();
    }
}
