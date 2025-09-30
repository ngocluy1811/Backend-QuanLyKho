using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models.DTOs
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateWarehouseDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Range(1, 50)]
        public int Width { get; set; }
        
        [Range(1, 50)]
        public int Height { get; set; }
    }

    public class UpdateWarehouseDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Range(1, 50)]
        public int Width { get; set; }
        
        [Range(1, 50)]
        public int Height { get; set; }
    }

    public class WarehousePositionDto
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string? ProductName { get; set; }
        public string? BatchNumber { get; set; }
        public int? MaxCapacity { get; set; }
        public int? CurrentAmount { get; set; }
        public string Status { get; set; } = "Empty"; // Empty, Occupied, Full, Reserved
        public string? ClusterName { get; set; }
        public DateTime? LastMoved { get; set; }
    }

    public class CreatePositionDto
    {
        [Required]
        public int WarehouseId { get; set; }
        
        [Range(0, 49)]
        public int Row { get; set; }
        
        [Range(0, 49)]
        public int Column { get; set; }
        
        [Range(1, 1000)]
        public int MaxCapacity { get; set; } = 100;
        
        public string? ClusterName { get; set; }
    }

    public class UpdatePositionDto
    {
        [Range(1, 1000)]
        public int? MaxCapacity { get; set; }
        
        public string? ProductName { get; set; }
        
        public string? BatchNumber { get; set; }
        
        [Range(0, 1000)]
        public int? CurrentAmount { get; set; }
        
        public string? ClusterName { get; set; }
    }

    public class WarehouseClusterDto
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PositionCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateClusterDto
    {
        [Required]
        public int WarehouseId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
